import { useEffect, useMemo, useState } from 'react'
import {
  CAlert,
  CBadge,
  CButton,
  CCard,
  CCardBody,
  CCardText,
  CCardTitle,
  CCol,
  CContainer,
  CFormSelect,
  CListGroup,
  CListGroupItem,
  CNav,
  CNavItem,
  CNavLink,
  CProgress,
  CProgressBar,
  CRow,
  CSpinner,
  CTable,
  CTableBody,
  CTableDataCell,
  CTableHead,
  CTableHeaderCell,
  CTableRow,
} from '@coreui/react'
import CIcon from '@coreui/icons-react'
import { cilClock, cilInfo, cilOptions } from '@coreui/icons'
import { eachDayOfInterval, endOfMonth, endOfWeek, format, startOfMonth, startOfWeek } from 'date-fns'

import './StudentAnalytics.css'

type Weekday = 'Mon' | 'Tue' | 'Wed' | 'Thu' | 'Fri'

type CourseBlock = {
  id: string
  subject: string
  day: Weekday
  startMinutes: number // minutes since 00:00
  endMinutes: number // minutes since 00:00
  color: 'info' | 'success' | 'warning' | 'danger' | 'primary'
}

type AttendanceItem = {
  label: string
  percent: number
  color: 'info' | 'primary' | 'success' | 'warning' | 'danger'
}

type SubjectPerfItem = {
  code: string
  percent: number
  color: 'info' | 'primary' | 'success' | 'warning' | 'danger'
}

type PopularTopic = {
  id: string
  label: string
  color: 'primary' | 'info' | 'success' | 'warning'
}

type StudentPerformanceDashboardData = {
  headerTitle: string
  gpa: number
  totalPassed: {
    passed: number
    total: number
  }
  calendar: {
    currentMonth: Date
    highlightedDays: Array<{ day: number; color: 'primary' | 'info' | 'success' | 'warning' }>
  }
  weeklySchedule: {
    startHour: number
    endHour: number
    blocks: CourseBlock[]
  }
  attendance: {
    contextLabel: string
    items: AttendanceItem[]
  }
  subjects: {
    contextLabel: string
    items: SubjectPerfItem[]
  }
  popularTopics: PopularTopic[]
}

const SIMULATE_ERROR = false
const MOCK_NETWORK_DELAY_MS = 650

async function mockLoadStudentPerformanceDashboard(): Promise<StudentPerformanceDashboardData> {
  await new Promise((r) => setTimeout(r, MOCK_NETWORK_DELAY_MS))

  if (SIMULATE_ERROR) {
    throw new Error('Failed to load analytics. Please try again.')
  }

  return {
    headerTitle: 'Performance dashboard',
    gpa: 8.5,
    totalPassed: { passed: 4, total: 10 },
    calendar: {
      currentMonth: new Date(2025, 10, 1), // November 2025
      highlightedDays: [
        { day: 29, color: 'primary' },
        { day: 5, color: 'primary' },
        { day: 13, color: 'info' },
        { day: 17, color: 'info' },
        { day: 20, color: 'info' },
        { day: 25, color: 'warning' },
        { day: 28, color: 'success' },
      ],
    },
    weeklySchedule: {
      startHour: 9,
      endHour: 18,
      blocks: [
        // close to the Figma screenshot timings (includes half-hour / non-even ends)
        { id: 'b1', subject: 'RSRV', day: 'Mon', startMinutes: 9 * 60 + 30, endMinutes: 11 * 60 + 20, color: 'primary' },
        { id: 'b2', subject: 'TS', day: 'Mon', startMinutes: 14 * 60, endMinutes: 17 * 60, color: 'info' },
        { id: 'b3', subject: 'MPVI', day: 'Tue', startMinutes: 12 * 60, endMinutes: 16 * 60, color: 'success' },
        { id: 'b4', subject: 'NSI', day: 'Wed', startMinutes: 12 * 60, endMinutes: 14 * 60, color: 'warning' },
        { id: 'b5', subject: 'IPMIS', day: 'Thu', startMinutes: 10 * 60, endMinutes: 14 * 60, color: 'danger' },
      ],
    },
    attendance: {
      contextLabel: 'for RSRV',
      items: [
        { label: 'Lectures', percent: 81.57, color: 'info' },
        { label: 'Tutorials', percent: 63.25, color: 'primary' },
      ],
    },
    subjects: {
      contextLabel: 'for current semester',
      items: [
        { code: 'RSRV', percent: 81.57, color: 'info' },
        { code: 'IPMIS', percent: 63.25, color: 'primary' },
        { code: 'NSI', percent: 52.95, color: 'warning' },
        { code: 'MPVI', percent: 47.29, color: 'success' },
        { code: 'TS', percent: 35.4, color: 'danger' },
      ],
    },
    popularTopics: [
      { id: 't1', label: 'Quiz', color: 'primary' },
      { id: 't2', label: 'Midterm', color: 'info' },
      { id: 't3', label: 'Exam', color: 'success' },
      { id: 't4', label: 'Public holiday', color: 'warning' },
    ],
  }
}

function clampPercent(n: number) {
  return Math.max(0, Math.min(100, n))
}

function makeCalendarMatrix(monthDate: Date) {
  const monthStart = startOfMonth(monthDate)
  const monthEnd = endOfMonth(monthDate)

  // Render a full calendar grid (includes previous/next month days) like the design
  const gridStart = startOfWeek(monthStart, { weekStartsOn: 0 }) // Sunday
  const gridEnd = endOfWeek(monthEnd, { weekStartsOn: 0 }) // Saturday
  const days = eachDayOfInterval({ start: gridStart, end: gridEnd })

  const weeks: Array<Array<Date>> = []
  for (let i = 0; i < days.length; i += 7) weeks.push(days.slice(i, i + 7))
  return weeks
}

function formatTime(minutes: number) {
  const h = Math.floor(minutes / 60)
  const m = minutes % 60
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`
}

export function StudentAnalytics() {
  const [data, setData] = useState<StudentPerformanceDashboardData | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [monthKey, setMonthKey] = useState('2025-11')
  const [attendanceContext, setAttendanceContext] = useState('RSRV')
  const [subjectsContext, setSubjectsContext] = useState('current')
  const [scheduleDay, setScheduleDay] = useState<Weekday>('Mon')

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        setLoading(true)
        setError(null)
        const result = await mockLoadStudentPerformanceDashboard()
        if (!cancelled) setData(result)
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'An unexpected error occurred.')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [])

  const monthDate = useMemo(() => {
    if (!data) return new Date(2025, 10, 1)
    const [y, m] = monthKey.split('-').map((x) => Number(x))
    if (!y || !m) return data.calendar.currentMonth
    return new Date(y, m - 1, 1)
  }, [data, monthKey])

  const calendarWeeks = useMemo(() => makeCalendarMatrix(monthDate), [monthDate])

  const highlightMap = useMemo(() => {
    const map = new Map<number, PopularTopic['color']>()
    for (const h of data?.calendar.highlightedDays ?? []) map.set(h.day, h.color)
    return map
  }, [data])

  const scheduleDays = useMemo(
    () =>
      [
        { day: 'Mon' as const, date: 10 },
        { day: 'Tue' as const, date: 11 },
        { day: 'Wed' as const, date: 12 },
        { day: 'Thu' as const, date: 13 },
        { day: 'Fri' as const, date: 14 },
      ],
    [],
  )

  const scheduleStartHour = useMemo(() => (data ? data.weeklySchedule.startHour : 9), [data])
  const scheduleEndHour = useMemo(() => (data ? data.weeklySchedule.endHour : 18), [data])
  const scheduleStartMin = useMemo(() => scheduleStartHour * 60, [scheduleStartHour])
  const blocksByDay = useMemo(() => {
    const map = new Map<Weekday, CourseBlock[]>()
    for (const d of ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'] as Weekday[]) map.set(d, [])
    for (const b of data?.weeklySchedule.blocks ?? []) map.get(b.day)?.push(b)
    for (const v of map.values()) v.sort((a, b) => a.startMinutes - b.startMinutes)
    return map
  }, [data])

  const scheduleRowHeightPx = 76
  const scheduleTotalHours = useMemo(() => {
    // +1 so the last tick (e.g. 18:00) is not flush with the bottom edge,
    // matching the Figma spacing and preventing blocks from visually overflowing.
    return Math.max(1, scheduleEndHour - scheduleStartHour + 1)
  }, [scheduleEndHour, scheduleStartHour])

  const scheduleTickHours = useMemo(() => {
    const ticks: number[] = []
    for (let h = scheduleStartHour; h <= scheduleEndHour; h++) ticks.push(h)
    return ticks
  }, [scheduleStartHour, scheduleEndHour])

  if (loading) {
    return (
      <CContainer fluid className="student-analytics-page student-analytics-loading">
        <CSpinner color="primary" />
      </CContainer>
    )
  }

  if (error) {
    return (
      <CContainer fluid className="student-analytics-page">
        <CAlert color="danger" className="student-analytics-alert">
          <h4 className="alert-heading">Error loading analytics</h4>
          <p className="mb-0">{error}</p>
        </CAlert>
      </CContainer>
    )
  }

  if (!data) return null

  return (
    <CContainer fluid className="student-analytics-page">
      <CRow className="mb-4">
        <CCol>
          <h1 className="student-analytics-title">{data.headerTitle}</h1>
        </CCol>
      </CRow>

      <CRow className="g-4">
        {/* LEFT */}
        <CCol xs={12} lg={8}>
          <CCard className="student-analytics-card student-analytics-card-lg mb-4">
            <CCardBody>
              <div className="student-analytics-card-header-center">
                <div className="student-analytics-card-eyebrow">WEEKLY COURSE SCHEDULE</div>
              </div>

              <div
                className="student-analytics-weekly-schedule"
                style={{
                  ['--sa-schedule-total-hours' as any]: scheduleTotalHours,
                  ['--sa-schedule-row-height' as any]: `${scheduleRowHeightPx}px`,
                }}
              >
                {/* Desktop/tablet schedule (full week grid) */}
                <div className="student-analytics-weekly-desktop">
                  <div className="student-analytics-weekly-schedule-header">
                    <div className="student-analytics-weekly-week-col">Week</div>
                    {scheduleDays.map((d) => (
                      <div key={d.day} className="student-analytics-weekly-day-head">
                        <div className="student-analytics-weekly-day-date">{d.date}</div>
                        <div className="student-analytics-weekly-day-name">{d.day}</div>
                      </div>
                    ))}
                  </div>

                  <div className="student-analytics-weekly-schedule-body">
                    <div className="student-analytics-weekly-time-col">
                      {scheduleTickHours.map((h) => (
                        <div
                          key={h}
                          className="student-analytics-weekly-time-tick"
                          style={{ top: (h - scheduleStartHour) * scheduleRowHeightPx }}
                        >
                          {String(h).padStart(2, '0')}:00
                        </div>
                      ))}
                    </div>

                    <div className="student-analytics-weekly-days-grid">
                      <div className="student-analytics-weekly-hour-lines" aria-hidden="true">
                        {scheduleTickHours.map((h) => (
                          <div
                            key={h}
                            className="student-analytics-weekly-hour-line"
                            style={{ top: (h - scheduleStartHour) * scheduleRowHeightPx }}
                          />
                        ))}
                      </div>
                      {scheduleDays.map((d) => (
                        <div key={d.day} className="student-analytics-weekly-day-col">
                          {(blocksByDay.get(d.day) ?? []).map((b) => {
                            const slotMinutes = 10
                            const startOffsetMin = Math.max(0, b.startMinutes - scheduleStartMin)
                            const endOffsetMin = Math.max(0, b.endMinutes - scheduleStartMin)

                            const startRow = Math.max(1, Math.round(startOffsetMin / slotMinutes) + 1)
                            const endRow = Math.max(startRow + 1, Math.round(endOffsetMin / slotMinutes) + 1)

                            return (
                              <CCard
                                key={b.id}
                                className={`student-analytics-weekly-event is-${b.subject}`}
                                style={{
                                  gridRow: `${startRow} / ${endRow}`,
                                }}
                              >
                                <CCardBody className="student-analytics-weekly-event-body">
                                  <div className="student-analytics-weekly-event-title">{b.subject}</div>
                                  <div className="student-analytics-weekly-event-time">
                                    <CIcon
                                      icon={cilClock}
                                      size="sm"
                                      className="student-analytics-weekly-event-clock"
                                    />
                                    {formatTime(b.startMinutes)} - {formatTime(b.endMinutes)}
                                  </div>
                                </CCardBody>
                              </CCard>
                            )
                          })}
                        </div>
                      ))}
                    </div>
                  </div>
                </div>

                {/* Mobile schedule: day tabs */}
                <div className="student-analytics-weekly-mobile">
                  <CNav variant="tabs" className="student-analytics-weekly-tabs">
                    {scheduleDays.map((d) => (
                      <CNavItem key={d.day}>
                        <CNavLink
                          active={scheduleDay === d.day}
                          onClick={() => setScheduleDay(d.day)}
                          className="student-analytics-weekly-tab"
                        >
                          {d.day}
                        </CNavLink>
                      </CNavItem>
                    ))}
                  </CNav>

                  <div className="student-analytics-weekly-schedule-body student-analytics-weekly-schedule-body-mobile">
                    <div className="student-analytics-weekly-time-col">
                      {scheduleTickHours.map((h) => (
                        <div
                          key={h}
                          className="student-analytics-weekly-time-tick"
                          style={{ top: (h - scheduleStartHour) * scheduleRowHeightPx }}
                        >
                          {String(h).padStart(2, '0')}:00
                        </div>
                      ))}
                    </div>

                    <div className="student-analytics-weekly-days-grid is-mobile">
                      <div className="student-analytics-weekly-hour-lines" aria-hidden="true">
                        {scheduleTickHours.map((h) => (
                          <div
                            key={h}
                            className="student-analytics-weekly-hour-line"
                            style={{ top: (h - scheduleStartHour) * scheduleRowHeightPx }}
                          />
                        ))}
                      </div>

                      <div className="student-analytics-weekly-day-col">
                        {(blocksByDay.get(scheduleDay) ?? []).map((b) => {
                          const slotMinutes = 10
                          const startOffsetMin = Math.max(0, b.startMinutes - scheduleStartMin)
                          const endOffsetMin = Math.max(0, b.endMinutes - scheduleStartMin)

                          const startRow = Math.max(1, Math.round(startOffsetMin / slotMinutes) + 1)
                          const endRow = Math.max(startRow + 1, Math.round(endOffsetMin / slotMinutes) + 1)

                          return (
                            <CCard
                              key={b.id}
                              className={`student-analytics-weekly-event is-${b.subject}`}
                              style={{ gridRow: `${startRow} / ${endRow}` }}
                            >
                              <CCardBody className="student-analytics-weekly-event-body">
                                <div className="student-analytics-weekly-event-title">{b.subject}</div>
                                <div className="student-analytics-weekly-event-time">
                                  <CIcon icon={cilClock} size="sm" className="student-analytics-weekly-event-clock" />
                                  {formatTime(b.startMinutes)} - {formatTime(b.endMinutes)}
                                </div>
                              </CCardBody>
                            </CCard>
                          )
                        })}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </CCardBody>
          </CCard>

          <CRow className="g-4">
            <CCol xs={12} md={6}>
              <CCard className="student-analytics-card h-100">
                <CCardBody>
                  <div className="student-analytics-attendance-header">
                    <div className="student-analytics-attendance-title">Attendance</div>
                    <div className="student-analytics-attendance-filter">
                      <span className="student-analytics-attendance-filter-label">for</span>
                      <CFormSelect
                        size="sm"
                        value={attendanceContext}
                        onChange={(e) => setAttendanceContext(e.target.value)}
                        aria-label="Attendance course filter"
                        className="student-analytics-attendance-select"
                      >
                        <option value="RSRV">RSRV</option>
                        <option value="IPMIS">IPMIS</option>
                        <option value="NSI">NSI</option>
                        <option value="MPVI">MPVI</option>
                        <option value="TS">TS</option>
                      </CFormSelect>
                    </div>
                  </div>

                  <div className="student-analytics-attendance-scale">
                    <div className="student-analytics-attendance-scale-track">
                      {['0%', '25%', '50%', '75%', '100%'].map((t) => (
                        <div key={t} className="student-analytics-attendance-tick">
                          {t}
                        </div>
                      ))}
                    </div>
                    <div className="student-analytics-attendance-scale-spacer" />
                  </div>

                  <div className="student-analytics-attendance-chart">
                    {/* Tutorials (purple) shown on top like the design */}
                    {(() => {
                      const tutorials = data.attendance.items.find((x) => x.label === 'Tutorials')
                      if (!tutorials) return null
                      return (
                        <div className="student-analytics-attendance-bar-row">
                          <CProgress className="student-analytics-attendance-progress">
                            <CProgressBar
                              value={clampPercent(tutorials.percent)}
                              className="student-analytics-attendance-bar student-analytics-attendance-bar-tutorials"
                            />
                          </CProgress>
                          <div className="student-analytics-attendance-bar-value">
                            {tutorials.percent.toFixed(0)}%
                          </div>
                        </div>
                      )
                    })()}

                    {(() => {
                      const lectures = data.attendance.items.find((x) => x.label === 'Lectures')
                      if (!lectures) return null
                      return (
                        <div className="student-analytics-attendance-bar-row">
                          <CProgress className="student-analytics-attendance-progress">
                            <CProgressBar
                              value={clampPercent(lectures.percent)}
                              className="student-analytics-attendance-bar student-analytics-attendance-bar-lectures"
                            />
                          </CProgress>
                          <div className="student-analytics-attendance-bar-value">
                            {lectures.percent.toFixed(0)}%
                          </div>
                        </div>
                      )
                    })()}
                  </div>

                  <CListGroup flush className="student-analytics-attendance-legend">
                    {data.attendance.items.map((item) => {
                      const key = item.label === 'Lectures' ? 'lectures' : 'tutorials'
                      return (
                        <CListGroupItem key={item.label} className="student-analytics-attendance-legend-item">
                          <div className="student-analytics-attendance-legend-left">
                            <CBadge className={`student-analytics-legend-dot is-${key}`}> </CBadge>
                            <span className="student-analytics-attendance-legend-label">{item.label}</span>
                          </div>
                          <div className="student-analytics-attendance-legend-value">
                            {item.percent.toFixed(2)}%
                          </div>
                        </CListGroupItem>
                      )
                    })}
                  </CListGroup>

                  <CCard className="student-analytics-mini-card">
                    <CCardBody className="student-analytics-mini-body">
                      <div className="student-analytics-mini-icon">
                        <CIcon icon={cilInfo} size="lg" />
                      </div>
                      <div>
                        <CCardTitle className="student-analytics-mini-title">Congratulations!</CCardTitle>
                        <CCardText className="student-analytics-mini-text">
                          Keep going,
                          <br />
                          you are a good student!
                        </CCardText>
                      </div>
                    </CCardBody>
                  </CCard>
                </CCardBody>
              </CCard>
            </CCol>

            <CCol xs={12} md={6}>
              <CCard className="student-analytics-card h-100">
                <CCardBody>
                  <div className="student-analytics-subjects-header">
                    <div className="student-analytics-subjects-title">Subjects</div>
                    <div className="student-analytics-subjects-filter">
                      <span className="student-analytics-subjects-filter-label">for</span>
                      <CFormSelect
                        size="sm"
                        value={subjectsContext}
                        onChange={(e) => setSubjectsContext(e.target.value)}
                        aria-label="Subjects period filter"
                        className="student-analytics-subjects-select"
                      >
                        <option value="current">current semester</option>
                        <option value="previous">previous semester</option>
                      </CFormSelect>
                    </div>
                  </div>

                  <div className="student-analytics-subjects-scale">
                    <div className="student-analytics-subjects-scale-track">
                      {['0%', '25%', '50%', '75%', '100%'].map((t) => (
                        <div key={t} className="student-analytics-subjects-tick">
                          {t}
                        </div>
                      ))}
                    </div>
                    <div className="student-analytics-subjects-scale-spacer" />
                  </div>

                  <div className="student-analytics-subjects-chart">
                    {(() => {
                      const order = ['IPMIS', 'MPVI', 'NSI', 'RSRV', 'TS']
                      const byCode = new Map(data.subjects.items.map((x) => [x.code, x]))
                      const ordered = order.map((c) => byCode.get(c)).filter(Boolean) as SubjectPerfItem[]

                      return ordered.map((item) => (
                        <div key={item.code} className="student-analytics-subjects-bar-row">
                          <CProgress className="student-analytics-subjects-progress">
                            <CProgressBar
                              value={clampPercent(item.percent)}
                              className={`student-analytics-subjects-bar is-${item.code}`}
                            />
                          </CProgress>
                          <div className="student-analytics-subjects-bar-value">
                            {item.percent.toFixed(0)}%
                          </div>
                        </div>
                      ))
                    })()}
                  </div>

                  <CListGroup flush className="student-analytics-subjects-legend">
                    {data.subjects.items.map((item) => (
                      <CListGroupItem key={item.code} className="student-analytics-subjects-legend-item">
                        <div className="student-analytics-subjects-legend-left">
                          <CBadge className={`student-analytics-legend-dot is-${item.code}`}> </CBadge>
                          <span className="student-analytics-subjects-legend-label">{item.code}</span>
                        </div>
                        <div className="student-analytics-subjects-legend-value">
                          {item.percent.toFixed(2)}%
                        </div>
                      </CListGroupItem>
                    ))}
                  </CListGroup>
                </CCardBody>
              </CCard>
            </CCol>
          </CRow>
        </CCol>

        {/* RIGHT */}
        <CCol xs={12} lg={4}>
          <CRow className="g-4 mb-4">
            <CCol xs={6}>
              <CCard className="student-analytics-card student-analytics-stat-card h-100">
                <CCardBody className="text-center">
                  <CCardTitle className="student-analytics-stat-value">{data.gpa}</CCardTitle>
                  <CCardText className="student-analytics-stat-label">GPA</CCardText>
                </CCardBody>
              </CCard>
            </CCol>
            <CCol xs={6}>
              <CCard className="student-analytics-card student-analytics-stat-card h-100">
                <CCardBody className="text-center">
                  <CCardTitle className="student-analytics-stat-value">
                    {data.totalPassed.passed}/{data.totalPassed.total}
                  </CCardTitle>
                  <CCardText className="student-analytics-stat-label">Total passed</CCardText>
                </CCardBody>
              </CCard>
            </CCol>
          </CRow>

          <CCard className="student-analytics-card mb-4">
            <CCardBody>
              <div className="student-analytics-calendar-header">
                <CFormSelect
                  size="sm"
                  aria-label="Select month"
                  value={monthKey}
                  onChange={(e) => setMonthKey(e.target.value)}
                  className="student-analytics-calendar-select"
                >
                  <option value="2025-11">November 2025</option>
                  <option value="2025-10">October 2025</option>
                  <option value="2025-12">December 2025</option>
                </CFormSelect>
              </div>

              <CTable borderless className="student-analytics-calendar-table mb-0">
                <CTableHead>
                  <CTableRow>
                    {['S', 'M', 'T', 'W', 'T', 'F', 'S'].map((d) => (
                      <CTableHeaderCell key={d} className="student-analytics-calendar-weekday">
                        {d}
                      </CTableHeaderCell>
                    ))}
                  </CTableRow>
                </CTableHead>
                <CTableBody>
                  {calendarWeeks.map((week, idx) => (
                    <CTableRow key={idx}>
                      {week.map((date, cidx) => {
                        const dayNum = Number(format(date, 'd'))
                        const isOutsideMonth = date.getMonth() !== monthDate.getMonth()
                        const highlightColor = !isOutsideMonth ? highlightMap.get(dayNum) : undefined

                        return (
                          <CTableDataCell key={cidx} className="student-analytics-calendar-cell">
                            <CButton
                              type="button"
                              size="sm"
                              color={highlightColor ?? 'light'}
                              className={[
                                'student-analytics-calendar-day',
                                isOutsideMonth ? 'is-outside' : '',
                                highlightColor ? 'is-highlight' : 'is-normal',
                              ].join(' ')}
                            >
                              {format(date, 'd')}
                            </CButton>
                          </CTableDataCell>
                        )
                      })}
                    </CTableRow>
                  ))}
                </CTableBody>
              </CTable>

              <div role="separator" className="student-analytics-side-divider" />

              <div className="student-analytics-popular-header">
                <div className="student-analytics-card-title">Popular Topics</div>
                <CButton color="link" size="sm" className="student-analytics-link-btn">
                  view details
                </CButton>
              </div>

              <CListGroup className="student-analytics-popular-list">
                {data.popularTopics.map((t) => (
                  <CListGroupItem key={t.id} className="student-analytics-popular-item">
                    <div className="student-analytics-popular-left">
                      <span className={`student-analytics-topic-dot bg-${t.color}`} />
                      <span className="student-analytics-topic-label">{t.label}</span>
                    </div>
                    <CButton
                      color="link"
                      className="student-analytics-topic-more"
                      aria-label="Topic options"
                    >
                      <CIcon icon={cilOptions} />
                    </CButton>
                  </CListGroupItem>
                ))}
              </CListGroup>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </CContainer>
  )
}