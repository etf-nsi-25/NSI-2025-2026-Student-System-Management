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
  CModal,
  CModalBody,
  CModalFooter,
  CModalHeader,
  CModalTitle,
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
import { eachDayOfInterval, endOfMonth, endOfWeek, format, startOfMonth, startOfWeek, addWeeks, subWeeks } from 'date-fns'
import { useAPI } from '../../context/services'
import { extractApiErrorMessage } from '../../utils/apiError'
import type { StudentAttendanceStatsDTO, SubjectProgressDTO, WeeklyScheduleDTO } from '../../dto/StudentAnalyticsDTO'
import type { Course } from '../../component/faculty/courses/types/Course'

import './StudentAnalytics.css'

type Weekday = 'Mon' | 'Tue' | 'Wed' | 'Thu' | 'Fri'

type CourseBlock = {
  id: string
  subject: string
  day: Weekday
  startMinutes: number // minutes since 00:00
  endMinutes: number // minutes since 00:00
  type?: string // "Lecture" or "Tutorial"
  color: 'info' | 'success' | 'warning' | 'danger' | 'primary' // Mapped on frontend
}

type AttendanceItem = {
  label: string // "Lectures" or "Tutorials"
  percent: number
  presentCount: number // Number of present/late attendances
  totalCount: number // Total number of attendances for this type
}

type SubjectPerfItem = {
  code: string
  percent: number
  color: 'info' | 'primary' | 'success' | 'warning' | 'danger'
  status: string
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
    highlightedDays: Array<{ 
      day: number
      eventType: 'Exam' | 'Assignment' | 'Midterm' | 'Quiz' | 'PublicHoliday'
      eventName?: string
      courseCode?: string
      color: 'primary' | 'info' | 'success' | 'warning'
    }>
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

async function loadStudentPerformanceDashboard(
  api: ReturnType<typeof useAPI>,
  monthKey: string,
  _attendanceContext: string,
  _subjectsContext: string
): Promise<StudentPerformanceDashboardData> {
  const [year, month] = monthKey.split('-').map(Number)
  
  const today = new Date()
  const currentWeekMonday = new Date(today)
  const dayOfWeek = today.getDay()
  const diff = today.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1)
  currentWeekMonday.setDate(diff)
  
  const [summary, calendar] = await Promise.all([
    api.getStudentSummary(),
    api.getMonthlyCalendar(year, month),
  ])

  const attendance: StudentAttendanceStatsDTO = { contextLabel: `for ${_attendanceContext}`, items: [] }

  return {
    headerTitle: 'Performance dashboard',
    gpa: summary.gpa ?? 0,
    totalPassed: { passed: summary.passedSubjects, total: summary.totalSubjects },
    calendar: {
      currentMonth: new Date(calendar.currentMonth),
      highlightedDays: calendar.highlightedDays.map(h => {
        const colorMap: Record<string, 'primary' | 'info' | 'success' | 'warning'> = {
          'Exam': 'success',
          'Assignment': 'info',
          'Midterm': 'info',
          'Quiz': 'warning',
          'PublicHoliday': 'warning',
        };
        
        const eventType = (h.eventType || 'Exam') as 'Exam' | 'Assignment' | 'Midterm' | 'Quiz' | 'PublicHoliday';
        const color = colorMap[eventType] || 'primary';
        
        return {
          day: h.day,
          eventType: eventType,
          eventName: h.eventName,
          courseCode: h.courseCode,
          color: color,
        };
      }),
    },
    weeklySchedule: {
      startHour: 9,
      endHour: 18,
      blocks: [],
    },
    attendance: {
      contextLabel: attendance.contextLabel,
      items: attendance.items.map(i => ({
        label: i.label,
        percent: i.percent,
        presentCount: i.presentCount,
        totalCount: i.totalCount,
      })),
    },
    subjects: {
      contextLabel: '',
      items: [],
    },
    popularTopics: [],
  }
}

function clampPercent(n: number) {
  return Math.max(0, Math.min(100, n))
}

function makeCalendarMatrix(monthDate: Date) {
  const monthStart = startOfMonth(monthDate)
  const monthEnd = endOfMonth(monthDate)

  const gridStart = startOfWeek(monthStart, { weekStartsOn: 0 })
  const gridEnd = endOfWeek(monthEnd, { weekStartsOn: 0 })
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

function handleAnalyticsError(error: unknown): string {
  const errorObj = error as { status?: number; message?: unknown }
  const status = errorObj?.status
  const rawMessage = extractApiErrorMessage(error)

  switch (status) {
    case 401:
      return 'Your session has expired. Please log in again to view your analytics.'
    case 403:
      return 'You do not have permission to access this data. Please contact support if you believe this is an error.'
    case 404:
      return 'Student data not found. Please ensure you are enrolled in courses.'
    case 500:
    case 502:
    case 503:
      return 'The server is experiencing issues. Please try again in a few moments.'
    default:
      if (rawMessage && rawMessage !== 'Unexpected error.') {
        return rawMessage
      }
      return 'Unable to load analytics data. Please refresh the page or try again later.'
  }
}

export function StudentAnalytics() {
  const api = useAPI()
  const [data, setData] = useState<StudentPerformanceDashboardData | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [monthKey, setMonthKey] = useState(() => {
    const now = new Date()
    return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
  })
  const [attendanceContext, setAttendanceContext] = useState<string>('')
  const [subjectsContext, setSubjectsContext] = useState('current')
  const [scheduleDay, setScheduleDay] = useState<Weekday>('Mon')

  const [selectedWeekStart, setSelectedWeekStart] = useState<Date>(() => {
    const today = new Date()
    const dayOfWeek = today.getDay()
    const diff = today.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1)
    const monday = new Date(today)
    monday.setDate(diff)
    return monday
  })
  const [scheduleData, setScheduleData] = useState<WeeklyScheduleDTO | null>(null)
  const [scheduleLoading, setScheduleLoading] = useState(false)
  const [scheduleError, setScheduleError] = useState<string | null>(null)
  
  const [selectedEvent, setSelectedEvent] = useState<{
    day: number
    eventType: string
    eventName?: string
    courseCode?: string
    date: Date
  } | null>(null)
  const [showEventModal, setShowEventModal] = useState(false)
  
  const [attendanceData, setAttendanceData] = useState<StudentAttendanceStatsDTO | null>(null)
  const [attendanceLoading, setAttendanceLoading] = useState(false)
  const [attendanceError, setAttendanceError] = useState<string | null>(null)
  
  const [subjectsData, setSubjectsData] = useState<SubjectProgressDTO | null>(null)
  const [subjectsLoading, setSubjectsLoading] = useState(false)
  const [subjectsError, setSubjectsError] = useState<string | null>(null)
  const [courses, setCourses] = useState<Course[]>([])

  useEffect(() => {
    let cancelled = false

    async function loadCourses() {
      try {
        const allCourses = await api.getAllCourses()
        if (!cancelled) {
          setCourses(allCourses)
          if (allCourses.length > 0 && attendanceContext === '') {
            setAttendanceContext(allCourses[0].code)
          }
        }
      } catch {
        if (!cancelled && attendanceContext === '' && courses.length > 0) {
          setAttendanceContext(courses[0].code)
        }
      }
    }

    loadCourses()
    return () => {
      cancelled = true
    }
  }, [api])

  useEffect(() => {
    let cancelled = false

    async function loadSchedule() {
      try {
        setScheduleLoading(true)
        setScheduleError(null)
        const result = await api.getWeeklySchedule(selectedWeekStart.toISOString())
        if (!cancelled) {
          setScheduleData(result)
          setScheduleError(null)
        }
      } catch (e) {
        if (!cancelled) {
          const userFriendlyMessage = handleAnalyticsError(e)
          setScheduleError(userFriendlyMessage)
          setScheduleData(null)
        }
      } finally {
        if (!cancelled) setScheduleLoading(false)
      }
    }

    loadSchedule()
    return () => {
      cancelled = true
    }
  }, [api, selectedWeekStart])

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        setLoading(true)
        setError(null)
        const result = await loadStudentPerformanceDashboard(api, monthKey, attendanceContext, subjectsContext)
        if (!cancelled) {
          setData(result)
          setError(null)
        }
      } catch (e) {
        if (!cancelled) {
          const userFriendlyMessage = handleAnalyticsError(e)
          setError(userFriendlyMessage)
          const errorObj = e as { status?: number }
          if (errorObj?.status === 401 || errorObj?.status === 403) {
            setData(null)
          }
        }
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [api, monthKey, subjectsContext])

  useEffect(() => {
    let cancelled = false

    async function loadAttendance() {
      if (!attendanceContext || courses.length === 0) {
        setAttendanceData(null)
        return
      }

      try {
        setAttendanceLoading(true)
        setAttendanceError(null)
        
        const course = courses.find(c => c.code === attendanceContext)
        
        if (!course) {
          if (!cancelled) {
            setAttendanceData({ contextLabel: `for ${attendanceContext}`, items: [] })
            setAttendanceError(null)
          }
          return
        }

        const result = await api.getAttendanceStats(course.id)
        if (!cancelled) {
          setAttendanceData(result)
          setAttendanceError(null)
        }
      } catch (e) {
        if (!cancelled) {
          const userFriendlyMessage = handleAnalyticsError(e)
          setAttendanceError(userFriendlyMessage)
          setAttendanceData({ contextLabel: `for ${attendanceContext}`, items: [] })
        }
      } finally {
        if (!cancelled) setAttendanceLoading(false)
      }
    }

    loadAttendance()
    return () => {
      cancelled = true
    }
  }, [api, attendanceContext, courses])

  useEffect(() => {
    let cancelled = false

    async function loadSubjects() {
      try {
        setSubjectsLoading(true)
        setSubjectsError(null)
        
        const result = await api.getSubjectProgress(undefined)
        
        const sortedResult = {
          ...result,
          items: [...result.items].sort((a, b) => b.percent - a.percent)
        }
        
        if (!cancelled) {
          setSubjectsData(sortedResult)
          setSubjectsError(null)
        }
      } catch (e) {
        if (!cancelled) {
          const userFriendlyMessage = handleAnalyticsError(e)
          setSubjectsError(userFriendlyMessage)
          setSubjectsData({ contextLabel: `for ${subjectsContext} semester`, items: [] })
        }
      } finally {
        if (!cancelled) {
          setSubjectsLoading(false)
        }
      }
    }

    loadSubjects()

    return () => {
      cancelled = true
    }
  }, [api, subjectsContext])

  const monthDate = useMemo(() => {
    if (!data) return new Date(2025, 10, 1)
    const [y, m] = monthKey.split('-').map((x) => Number(x))
    if (!y || !m) return data.calendar.currentMonth
    return new Date(y, m - 1, 1)
  }, [data, monthKey])

  const calendarWeeks = useMemo(() => makeCalendarMatrix(monthDate), [monthDate])

  const highlightMap = useMemo(() => {
    const map = new Map<number, { color: 'primary' | 'info' | 'success' | 'warning', eventType: string, eventName?: string, courseCode?: string }>()
    for (const h of data?.calendar.highlightedDays ?? []) {
      map.set(h.day, {
        color: h.color,
        eventType: h.eventType,
        eventName: h.eventName,
        courseCode: h.courseCode
      })
    }
    return map
  }, [data])

  const monthOptions = useMemo(() => {
    const options: Array<{ value: string; label: string }> = []
    const now = new Date()
    const currentYear = now.getFullYear()
    const currentMonth = now.getMonth() + 1

    for (let i = -6; i <= 6; i++) {
      const date = new Date(currentYear, currentMonth - 1 + i, 1)
      const year = date.getFullYear()
      const month = date.getMonth() + 1
      const monthKey = `${year}-${String(month).padStart(2, '0')}`
      const monthName = format(date, 'MMMM yyyy')
      options.push({ value: monthKey, label: monthName })
    }

    return options
  }, [])

  const scheduleDays = useMemo(() => {
    const days: Array<{ day: Weekday; date: number }> = []
    const weekdays: Weekday[] = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri']
    
    for (let i = 0; i < 5; i++) {
      const date = new Date(selectedWeekStart)
      date.setDate(selectedWeekStart.getDate() + i)
      days.push({
        day: weekdays[i],
        date: date.getDate(),
      })
    }
    
    return days
  }, [selectedWeekStart])

  const currentSchedule = scheduleData || data?.weeklySchedule
  const scheduleStartHour = useMemo(() => (currentSchedule ? currentSchedule.startHour : 9), [currentSchedule])
  const scheduleEndHour = useMemo(() => (currentSchedule ? currentSchedule.endHour : 18), [currentSchedule])
  const scheduleStartMin = useMemo(() => scheduleStartHour * 60, [scheduleStartHour])
  const blocksByDay = useMemo(() => {
    const map = new Map<Weekday, CourseBlock[]>()
    for (const d of ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'] as Weekday[]) map.set(d, [])
    
    const getColorFromCode = (code: string): 'info' | 'primary' | 'success' | 'warning' | 'danger' => {
      const colors: ('info' | 'primary' | 'success' | 'warning' | 'danger')[] = [
        'primary',
        'success',
        'warning',
        'info',
        'danger',
      ]
      const hash = code.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
      return colors[Math.abs(hash) % colors.length]
    }
    
    const blocks = currentSchedule?.blocks ?? []
    for (const b of blocks) {
      const day = b.day as Weekday
      const color = getColorFromCode(b.subject)
      if (map.has(day)) {
        map.get(day)?.push({
          id: b.id,
          subject: b.subject,
          day: day,
          startMinutes: b.startMinutes,
          endMinutes: b.endMinutes,
          type: b.type,
          color: color,
        })
      }
    }
    
    for (const v of map.values()) v.sort((a, b) => a.startMinutes - b.startMinutes)
    return map
  }, [currentSchedule])

  const scheduleRowHeightPx = 76
  const scheduleTotalHours = useMemo(() => {
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

  if (error && !data) {
    return (
      <CContainer fluid className="student-analytics-page">
        <CAlert color="danger" className="student-analytics-alert">
          <h4 className="alert-heading">Unable to Load Analytics</h4>
          <p className="mb-0">{error}</p>
        </CAlert>
      </CContainer>
    )
  }

  if (!data && !loading) {
    return (
      <CContainer fluid className="student-analytics-page">
        <CAlert color="info" className="student-analytics-alert">
          <h4 className="alert-heading">No Data Available</h4>
          <p className="mb-0">No analytics data found. Please ensure you are enrolled in courses.</p>
        </CAlert>
      </CContainer>
    )
  }

  if (!data) return null

  return (
    <CContainer fluid className="student-analytics-page">
      {error && data && (
        <CRow className="mb-3">
          <CCol>
            <CAlert color="warning" dismissible onClose={() => setError(null)}>
              <strong>Warning:</strong> {error}
            </CAlert>
          </CCol>
        </CRow>
      )}
      
      <CRow className="mb-4">
        <CCol>
          <h1 className="student-analytics-title">{data.headerTitle}</h1>
        </CCol>
      </CRow>

      <CRow className="g-4">
        <CCol xs={12} lg={8}>
          <CCard className="student-analytics-card student-analytics-card-lg mb-4">
            <CCardBody>
              <div className="student-analytics-card-header-center">
                <div className="student-analytics-card-eyebrow">WEEKLY COURSE SCHEDULE</div>
                <div className="student-analytics-week-navigation">
                  <CButton
                    size="sm"
                    color="secondary"
                    variant="outline"
                    onClick={() => setSelectedWeekStart(subWeeks(selectedWeekStart, 1))}
                    disabled={scheduleLoading}
                  >
                    ← Previous
                  </CButton>
                  <span className="student-analytics-week-range">
                    {format(selectedWeekStart, 'MMM d')} - {format(new Date(selectedWeekStart.getTime() + 4 * 24 * 60 * 60 * 1000), 'MMM d')}, {format(selectedWeekStart, 'yyyy')}
                  </span>
                  <CButton
                    size="sm"
                    color="secondary"
                    variant="outline"
                    onClick={() => setSelectedWeekStart(addWeeks(selectedWeekStart, 1))}
                    disabled={scheduleLoading}
                  >
                    Next →
                  </CButton>
                  <CButton
                    size="sm"
                    color="link"
                    variant="ghost"
                    onClick={() => {
                      const today = new Date()
                      const dayOfWeek = today.getDay()
                      const diff = today.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1)
                      const monday = new Date(today)
                      monday.setDate(diff)
                      setSelectedWeekStart(monday)
                    }}
                    disabled={scheduleLoading}
                  >
                    Today
                  </CButton>
                </div>
              </div>

              {scheduleLoading && (
                <div className="student-analytics-widget-loading">
                  <CSpinner size="sm" color="primary" />
                  <p>Loading schedule...</p>
                </div>
              )}

              {scheduleError && !scheduleLoading && (
                <div className="student-analytics-widget-error">
                  <small>Unable to load schedule. {scheduleError}</small>
                </div>
              )}

              {!scheduleLoading && !scheduleError && currentSchedule && currentSchedule.blocks.length === 0 && (
                <div className="student-analytics-widget-empty">
                  <p>No classes scheduled for this week.</p>
                </div>
              )}

              {(!scheduleLoading || currentSchedule) && currentSchedule && currentSchedule.blocks.length > 0 && (
              <div
                className="student-analytics-weekly-schedule"
                style={{
                  ['--sa-schedule-total-hours' as any]: scheduleTotalHours,
                  ['--sa-schedule-row-height' as any]: `${scheduleRowHeightPx}px`,
                }}
              >
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

                            const colorMap: Record<string, { bg: string; border: string }> = {
                              primary: { bg: '#eef0f6', border: 'rgba(59, 130, 246, 0.3)' },
                              info: { bg: '#f1f8ea', border: 'rgba(34, 197, 94, 0.3)' },
                              success: { bg: '#fdf5e6', border: 'rgba(234, 179, 8, 0.3)' },
                              warning: { bg: '#fff7f7', border: 'rgba(239, 68, 68, 0.3)' },
                              danger: { bg: '#fbf7ff', border: 'rgba(139, 92, 246, 0.3)' },
                            }
                            const colors = colorMap[b.color] || colorMap.primary

                            return (
                              <CCard
                                key={b.id}
                                className="student-analytics-weekly-event"
                                style={{
                                  gridRow: `${startRow} / ${endRow}`,
                                  background: colors.bg,
                                  borderColor: colors.border,
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

                          const colorMap: Record<string, { bg: string; border: string }> = {
                            primary: { bg: '#eef0f6', border: 'rgba(59, 130, 246, 0.3)' },
                            info: { bg: '#f1f8ea', border: 'rgba(34, 197, 94, 0.3)' },
                            success: { bg: '#fdf5e6', border: 'rgba(234, 179, 8, 0.3)' },
                            warning: { bg: '#fff7f7', border: 'rgba(239, 68, 68, 0.3)' },
                            danger: { bg: '#fbf7ff', border: 'rgba(139, 92, 246, 0.3)' },
                          }
                          const colors = colorMap[b.color] || colorMap.primary

                          return (
                            <CCard
                              key={b.id}
                              className="student-analytics-weekly-event"
                              style={{
                                gridRow: `${startRow} / ${endRow}`,
                                background: colors.bg,
                                borderColor: colors.border,
                              }}
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
              )}
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
                        disabled={attendanceLoading}
                      >
                        {courses.length > 0 ? (
                          courses.map((course) => (
                            <option key={course.id} value={course.code}>
                              {course.code}
                            </option>
                          ))
                        ) : (
                          <option value="">No courses available</option>
                        )}
                      </CFormSelect>
                    </div>
                  </div>

                  {attendanceLoading && (
                    <div className="student-analytics-widget-loading">
                      <CSpinner size="sm" color="primary" />
                      <p>Loading attendance data...</p>
                    </div>
                  )}

                  {attendanceError && !attendanceLoading && (
                    <div className="student-analytics-widget-error">
                      <small>{attendanceError}</small>
                    </div>
                  )}

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

                  {/* Empty state */}
                  {!attendanceLoading && !attendanceError && attendanceData && attendanceData.items.length === 0 && (
                    <div className="student-analytics-widget-empty">
                      <p>No attendance data available for this course.</p>
                    </div>
                  )}

                  {!attendanceLoading && attendanceData && attendanceData.items.length > 0 && (
                    <>
                  <div className="student-analytics-attendance-chart">
                    {(() => {
                          const tutorials = attendanceData.items.find((x) => x.label === 'Tutorials')
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
                          const lectures = attendanceData.items.find((x) => x.label === 'Lectures')
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
                        {attendanceData.items.map((item) => {
                          const colorMap: Record<string, string> = {
                            'Lectures': '#3b82f6',
                            'Tutorials': '#8b5cf6',
                          }
                          const borderColor = colorMap[item.label] || '#3b82f6'

                      return (
                        <CListGroupItem key={item.label} className="student-analytics-attendance-legend-item">
                          <div className="student-analytics-attendance-legend-left">
                                <CBadge
                                  className="student-analytics-legend-dot"
                                  style={{ borderColor: borderColor }}
                                >
                                  {' '}
                                </CBadge>
                            <span className="student-analytics-attendance-legend-label">{item.label}</span>
                          </div>
                          <div className="student-analytics-attendance-legend-value">
                            {item.percent.toFixed(2)}%
                          </div>
                        </CListGroupItem>
                      )
                    })}
                  </CListGroup>
                    </>
                  )}

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
                        disabled={subjectsLoading}
                      >
                        <option value="current">current semester</option>
                        <option value="previous">previous semester</option>
                      </CFormSelect>
                    </div>
                  </div>

                  {subjectsLoading && (
                    <div className="student-analytics-widget-loading">
                      <CSpinner size="sm" color="primary" />
                      <p>Loading subjects progress...</p>
                    </div>
                  )}

                  {subjectsError && !subjectsLoading && (
                    <div className="student-analytics-widget-error">
                      <small>Unable to load subjects progress. {subjectsError}</small>
                    </div>
                  )}

                  {!subjectsLoading && !subjectsError && subjectsData && subjectsData.items.length === 0 && (
                    <div className="student-analytics-widget-empty">
                      <p>No subjects found for {subjectsData.contextLabel || 'selected period'}.</p>
                    </div>
                  )}

                  {!subjectsLoading && subjectsData && subjectsData.items.length > 0 && (
                    <>
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
                          if (!subjectsData) return null
                          const items = subjectsData.items
                          
                          const getColorFromCode = (code: string): string => {
                            const colors = [
                              '#3b82f6',
                              '#6cc6a7',
                              '#f8e7b2',
                              '#8b5cf6',
                              '#e6a27c',
                            ]
                            const hash = code.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
                            return colors[Math.abs(hash) % colors.length]
                          }

                          return items.map((item) => {
                            const bgColor = getColorFromCode(item.code)
                            return (
                        <div key={item.code} className="student-analytics-subjects-bar-row">
                          <CProgress className="student-analytics-subjects-progress">
                            <CProgressBar
                              value={clampPercent(item.percent)}
                                    className="student-analytics-subjects-bar"
                                    style={{
                                      backgroundColor: bgColor,
                                      '--cui-progress-bar-bg': bgColor,
                                    } as React.CSSProperties}
                            />
                          </CProgress>
                          <div className="student-analytics-subjects-bar-value">
                            {item.percent.toFixed(2)}%
                          </div>
                        </div>
                            )
                          })
                    })()}
                  </div>

                  <CListGroup flush className="student-analytics-subjects-legend">
                        {subjectsData?.items.map((item) => {
                          const getColorFromCode = (code: string): string => {
                            const colors = [
                              '#3b82f6',
                              '#6cc6a7',
                              '#f8e7b2',
                              '#8b5cf6',
                              '#e6a27c',
                            ]
                            const hash = code.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
                            return colors[Math.abs(hash) % colors.length]
                          }
                          const borderColor = getColorFromCode(item.code)

                          return (
                      <CListGroupItem key={item.code} className="student-analytics-subjects-legend-item">
                        <div className="student-analytics-subjects-legend-left">
                                <CBadge
                                  className="student-analytics-legend-dot"
                                  style={{ borderColor: borderColor }}
                                >
                                  {' '}
                                </CBadge>
                                <div className="d-flex flex-column">
                          <span className="student-analytics-subjects-legend-label">{item.code}</span>
                                  {item.status && (
                                    <span className="student-analytics-subjects-legend-status">{item.status}</span>
                                  )}
                                </div>
                        </div>
                        <div className="student-analytics-subjects-legend-value">
                          {item.percent.toFixed(2)}%
                        </div>
                      </CListGroupItem>
                          )
                        })}
                  </CListGroup>
                    </>
                  )}
                </CCardBody>
              </CCard>
            </CCol>
          </CRow>
        </CCol>

        <CCol xs={12} lg={4}>
          <CRow className="g-4 mb-4">
            <CCol xs={12} sm={6}>
              <CCard className="student-analytics-card student-analytics-stat-card h-100">
                <CCardBody className="text-center">
                  {loading ? (
                    <>
                      <CSpinner size="sm" color="primary" className="mb-2" />
                      <CCardText className="student-analytics-stat-label text-muted">Loading...</CCardText>
                    </>
                  ) : error && !data ? (
                    <>
                      <CCardTitle className="student-analytics-stat-value text-muted">—</CCardTitle>
                      <CCardText className="student-analytics-stat-label text-danger small">Error loading</CCardText>
                    </>
                  ) : !data || data.gpa === undefined || data.gpa === null ? (
                    <>
                      <CCardTitle className="student-analytics-stat-value text-muted">N/A</CCardTitle>
                  <CCardText className="student-analytics-stat-label">GPA</CCardText>
                    </>
                  ) : (
                    <>
                      <CCardTitle className="student-analytics-stat-value">
                        {data.gpa.toFixed(2)}
                      </CCardTitle>
                      <CCardText className="student-analytics-stat-label">GPA</CCardText>
                    </>
                  )}
                </CCardBody>
              </CCard>
            </CCol>
            
            <CCol xs={12} sm={6}>
              <CCard className="student-analytics-card student-analytics-stat-card h-100">
                <CCardBody className="text-center">
                  {loading ? (
                    <>
                      <CSpinner size="sm" color="primary" className="mb-2" />
                      <CCardText className="student-analytics-stat-label text-muted">Loading...</CCardText>
                    </>
                  ) : error && !data ? (
                    <>
                      <CCardTitle className="student-analytics-stat-value text-muted">—</CCardTitle>
                      <CCardText className="student-analytics-stat-label text-danger small">Error loading</CCardText>
                    </>
                  ) : !data || !data.totalPassed ? (
                    <>
                      <CCardTitle className="student-analytics-stat-value text-muted">N/A</CCardTitle>
                      <CCardText className="student-analytics-stat-label">Total passed</CCardText>
                    </>
                  ) : (
                    <>
                  <CCardTitle className="student-analytics-stat-value">
                    {data.totalPassed.passed}/{data.totalPassed.total}
                  </CCardTitle>
                  <CCardText className="student-analytics-stat-label">Total passed</CCardText>
                    </>
                  )}
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
                  disabled={loading}
                >
                  {monthOptions.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </CFormSelect>
              </div>

              {loading && (
                <div className="student-analytics-widget-loading">
                  <CSpinner size="sm" color="primary" />
                  <p>Loading calendar events...</p>
                </div>
              )}

              {error && !loading && (
                <div className="student-analytics-widget-error">
                  <small>Unable to load calendar events. {error}</small>
                </div>
              )}

              {(!loading || data) && (
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
                        const highlightInfo = !isOutsideMonth ? highlightMap.get(dayNum) : undefined
                        const highlightColor = highlightInfo?.color
                        const tooltipText = highlightInfo 
                          ? `${highlightInfo.eventType}${highlightInfo.courseCode ? ` - ${highlightInfo.courseCode}` : ''}${highlightInfo.eventName ? `: ${highlightInfo.eventName}` : ''}`
                          : undefined

                        const handleDayClick = () => {
                          if (highlightInfo && !isOutsideMonth) {
                            setSelectedEvent({
                              day: dayNum,
                              eventType: highlightInfo.eventType,
                              eventName: highlightInfo.eventName,
                              courseCode: highlightInfo.courseCode,
                              date: date
                            })
                            setShowEventModal(true)
                          }
                        }

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
                                highlightInfo ? 'cursor-pointer' : '',
                              ].join(' ')}
                              title={tooltipText}
                              onClick={handleDayClick}
                              disabled={!highlightInfo || isOutsideMonth}
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
              )}

              {!loading && !error && data && data.calendar.highlightedDays.length === 0 && (
                <div className="student-analytics-widget-empty">
                  <p>No events scheduled for this month.</p>
                </div>
              )}

              <div role="separator" className="student-analytics-side-divider" />

              {/* Popular Topics section - Legend for calendar event types */}
              <div className="student-analytics-popular-header">
                <div className="student-analytics-card-title">Popular Topics</div>
                <CButton color="link" size="sm" className="student-analytics-link-btn">
                  View details
                </CButton>
              </div>

              <CListGroup className="student-analytics-popular-list">
                <CListGroupItem className="student-analytics-popular-item">
                  <div className="student-analytics-popular-left">
                    <span className="student-analytics-topic-dot" style={{ backgroundColor: '#a78bfa' }} />
                    <span className="student-analytics-topic-label">Quiz</span>
                  </div>
                  <CButton
                    color="link"
                    className="student-analytics-topic-more"
                    aria-label="Topic options"
                  >
                    <CIcon icon={cilOptions} />
                  </CButton>
                </CListGroupItem>
                
                <CListGroupItem className="student-analytics-popular-item">
                  <div className="student-analytics-popular-left">
                    <span className="student-analytics-topic-dot bg-info" />
                    <span className="student-analytics-topic-label">Midterm</span>
                  </div>
                  <CButton
                    color="link"
                    className="student-analytics-topic-more"
                    aria-label="Topic options"
                  >
                    <CIcon icon={cilOptions} />
                  </CButton>
                </CListGroupItem>
                
                <CListGroupItem className="student-analytics-popular-item">
                  <div className="student-analytics-popular-left">
                    <span className="student-analytics-topic-dot bg-success" />
                    <span className="student-analytics-topic-label">Exam</span>
                  </div>
                  <CButton
                    color="link"
                    className="student-analytics-topic-more"
                    aria-label="Topic options"
                  >
                    <CIcon icon={cilOptions} />
                  </CButton>
                </CListGroupItem>
                
                <CListGroupItem className="student-analytics-popular-item">
                  <div className="student-analytics-popular-left">
                    <span className="student-analytics-topic-dot bg-warning" />
                    <span className="student-analytics-topic-label">Public holiday</span>
                  </div>
                  <CButton
                    color="link"
                    className="student-analytics-topic-more"
                    aria-label="Topic options"
                  >
                    <CIcon icon={cilOptions} />
                  </CButton>
                </CListGroupItem>
              </CListGroup>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>

      <CModal 
        visible={showEventModal} 
        onClose={() => setShowEventModal(false)} 
        alignment="center" 
        size="sm"
        className="modal-super-high-zindex"
      >
        <CModalHeader closeButton>
          <CModalTitle>Event Details</CModalTitle>
        </CModalHeader>
        <CModalBody>
          {selectedEvent && (
            <div>
              <div className="mb-3">
                <div className="d-flex align-items-center gap-2 mb-2">
                  <CBadge 
                    color={selectedEvent.eventType === 'Exam' ? 'success' : selectedEvent.eventType === 'Assignment' ? 'info' : 'warning'}
                    className="px-3 py-2"
                  >
                    {selectedEvent.eventType}
                  </CBadge>
                  {selectedEvent.courseCode && (
                    <span className="fw-bold text-muted">{selectedEvent.courseCode}</span>
                  )}
                </div>
                {selectedEvent.eventName && (
                  <h5 className="mb-2">{selectedEvent.eventName}</h5>
                )}
                <div className="text-muted small">
                  <div className="mb-1">
                    <strong>Date:</strong> {format(selectedEvent.date, 'EEEE, MMMM d, yyyy')}
                  </div>
                  {selectedEvent.courseCode && (
                    <div>
                      <strong>Course:</strong> {selectedEvent.courseCode}
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}
        </CModalBody>
        <CModalFooter>
          <CButton color="secondary" onClick={() => setShowEventModal(false)}>
            Close
          </CButton>
        </CModalFooter>
      </CModal>
    </CContainer>
  )
}