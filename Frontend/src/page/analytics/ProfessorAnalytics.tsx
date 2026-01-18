import { useMemo, useState, useEffect } from "react";
import { useAPI } from "../../context/services";
import { GradesOverview } from "../../component/analytics/GradesOverview";
import { fetchGrades } from "../../component/analytics/mockGradesService";
import "./ProfessorAnalytics.css";

type CalendarLegendItem = {
  label: string;
  className: string;
};

const WEEKDAYS = ["s", "M", "T", "W", "t", "F", "S"];

function startOfMonth(d: Date) {
  return new Date(d.getFullYear(), d.getMonth(), 1);
}
function endOfMonth(d: Date) {
  return new Date(d.getFullYear(), d.getMonth() + 1, 0);
}
function isSameDay(a: Date, b: Date) {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  );
}
function addMonths(d: Date, delta: number) {
  return new Date(d.getFullYear(), d.getMonth() + delta, 1);
}
function clampToMonth(date: Date, monthDate: Date) {
  if (date.getFullYear() !== monthDate.getFullYear() || date.getMonth() !== monthDate.getMonth()) {
    return new Date(monthDate.getFullYear(), monthDate.getMonth(), 1);
  }
  return date;
}

export type GradeSlice = { label: string; value: number; color: string };

export const COLORS = {
  low: '#2f3abf',
  six: '#f093c6',
  seven: '#e93aa0',
  eight: '#19c7e6',
  nine: '#a7f06a',
  ten: '#e6db39',
};

export default function ProfessorAnalyticsPage() {

  const [monthCursor, setMonthCursor] = useState<Date>(() => startOfMonth(new Date(2025, 10, 1))); // Nov 2025
  const [selectedDate, setSelectedDate] = useState<Date>(() => new Date(2025, 10, 13));

  const markers = useMemo(() => {
    const m = new Map<string, string>();
    const key = (y: number, mo: number, d: number) =>
      `${y}-${String(mo + 1).padStart(2, "0")}-${String(d).padStart(2, "0")}`;

    m.set(key(2025, 10, 5), "quiz");
    m.set(key(2025, 10, 13), "midterm");
    m.set(key(2025, 10, 17), "other");
    m.set(key(2025, 10, 20), "midterm");
    m.set(key(2025, 10, 25), "quiz");
    return m;
  }, []);

  const legend: CalendarLegendItem[] = [
    { label: "Quiz", className: "calDot calDot--quiz" },
    { label: "Midterm", className: "calDot calDot--midterm" },
  ];

  const monthLabel = useMemo(
    () => monthCursor.toLocaleString(undefined, { month: "long", year: "numeric" }),
    [monthCursor]
  );

  const calendarCells = useMemo(() => {
    const first = startOfMonth(monthCursor);
    const last = endOfMonth(monthCursor);

    const startWeekday = first.getDay(); // 0=Sun
    const totalDays = last.getDate();

    const cells: Array<{ date: Date; inMonth: boolean }> = [];

    // Prev month padding
    const prevMonthLast = new Date(first.getFullYear(), first.getMonth(), 0);
    const prevDays = prevMonthLast.getDate();
    for (let i = startWeekday - 1; i >= 0; i--) {
      cells.push({
        date: new Date(first.getFullYear(), first.getMonth() - 1, prevDays - i),
        inMonth: false,
      });
    }

    // Current month days
    for (let d = 1; d <= totalDays; d++) {
      cells.push({
        date: new Date(first.getFullYear(), first.getMonth(), d),
        inMonth: true,
      });
    }

    // Next month padding to 42 (stable 6 rows)
    while (cells.length < 42) {
      const idx = cells.length - (startWeekday + totalDays);
      cells.push({
        date: new Date(first.getFullYear(), first.getMonth() + 1, idx + 1),
        inMonth: false,
      });
    }

    return cells;
  }, [monthCursor]);

  const markerTypeFor = (d: Date) => {
    const k = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
    return markers.get(k);
  };

  const goPrev = () => {
    const next = addMonths(monthCursor, -1);
    setMonthCursor(next);
    setSelectedDate((prev) => clampToMonth(prev, next));
  };

  const goNext = () => {
    const next = addMonths(monthCursor, +1);
    setMonthCursor(next);
    setSelectedDate((prev) => clampToMonth(prev, next));
  };

  const api = useAPI();
  const [academicYears, setAcademicYears] = useState<string[]>([]);
  const [courses, setCourses] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const [gradeData, setGradeData] = useState<GradeSlice[]>([]);
  const [gradesLoading, setGradesLoading] = useState(false);

  const [selectedYear, setSelectedYear] = useState("");
  const [selectedCourse, setSelectedCourse] = useState("");
  const [selectedSemester, setSelectedSemester] = useState("1. semester");
  const semesters = ["1. semester", "2. semester", "3. semester", "4. semester", "5. semester", "6. semester"];

  useEffect(() => {
    const fetchFilterData = async () => {
      if (!api) return;

      try {
        setIsLoading(true);

        const data = await api.getTeacherFilterData();

        if (data) {
          setAcademicYears(data.years || []);
          setCourses(data.courses || []);

          if (data.years?.length) setSelectedYear(data.years[0]);
          if (data.courses?.length) setSelectedCourse(data.courses[0]);
        }
      } catch (error) {
        console.error("Greška:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchFilterData();
  }, []);

  useEffect(() => {
    const fetch = async () => {
      if (!selectedCourse) return;
      setGradesLoading(true);

      // Try backend first. backend returns { courseId, distribution: {"5": 12, ...}, totalCount, passedCount }
      try {
        if (api) {
          const res = await api.get<any>(`/api/stats/course/${selectedCourse}`);
          if (res) {
            const dist = res.distribution || res.distribution || res?.Distribution || res?.distribution;
            const colorList = Object.values(COLORS);
            const slices: GradeSlice[] = Object.keys(dist || {}).map((k, i) => ({ label: k, value: dist[k], color: colorList[i % colorList.length] }));
            setGradeData(slices.length ? slices : await fetchGrades("all"));
            setGradesLoading(false);
            return;
          }
        }
      } catch (e) {
        // fallback to mock
      }

      // fallback to mock data depending on semester selection
      const f = selectedSemester?.toLowerCase()?.includes('midterm') ? 'midterm' : 'final';
      const mock = await fetchGrades(f);
      setGradeData(mock);
      setGradesLoading(false);
    };

    fetch();
  }, [selectedCourse, selectedYear, selectedSemester, api]);

  if (isLoading) {
    return (
      <div className="paPage" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <h3>Učitavanje podataka...</h3>
      </div>
    );
  }
  return (
    <div className="paPage">
      {/* LEFT SIDEBAR (dropdowns + calendar) */}
      <aside className="paSidebar">
        <div className="paSelectStack">
          {/* Godina */}
          <div className="paFakeSelect">
            <select
              className="paActualSelect"
              value={selectedYear}
              onChange={(e) => setSelectedYear(e.target.value)}
            >
              {academicYears.map(year => (
                <option key={year} value={year}>{year}</option>
              ))}
            </select>
            <span className="paFakeSelect__value">{selectedYear}</span>
            <span className="paFakeSelect__chev">▾</span>
          </div>

          {/* Semestar */}
          <div className="paFakeSelect">
            <select
              className="paActualSelect"
              value={selectedSemester}
              onChange={(e) => setSelectedSemester(e.target.value)}
            >
              {semesters.map(sem => (
                <option key={sem} value={sem}>{sem}</option>
              ))}
            </select>
            <span className="paFakeSelect__value">{selectedSemester}</span>
            <span className="paFakeSelect__chev">▾</span>
          </div>

          {/* Kurs */}
          <div className="paFakeSelect">
            <select
              className="paActualSelect"
              value={selectedCourse}
              onChange={(e) => setSelectedCourse(e.target.value)}
            >
              {courses.map(course => (
                <option key={course} value={course}>{course}</option>
              ))}
            </select>
            <span className="paFakeSelect__value">{selectedCourse}</span>
            <span className="paFakeSelect__chev">▾</span>
          </div>
        </div>

        {/*Calendar*/}
        <section className="paCalendarCard">
          <div className="paCalendarHeader">
            <button className="paIconBtn" onClick={goPrev} aria-label="Previous month">
              ‹
            </button>
            <div className="paCalendarTitle">{monthLabel}</div>
            <button className="paIconBtn" onClick={goNext} aria-label="Next month">
              ›
            </button>
          </div>

          <div className="paCalendarGrid">
            {WEEKDAYS.map((w) => (
              <div key={w} className="paCalendarWeekday">
                {w}
              </div>
            ))}

            {calendarCells.map(({ date, inMonth }, idx) => {
              const marker = markerTypeFor(date);
              const isSelected = isSameDay(date, selectedDate);
              const isToday = isSameDay(date, new Date());

              return (
                <button
                  key={`${date.toISOString()}-${idx}`}
                  className={[
                    "paDay",
                    inMonth ? "isInMonth" : "isOutMonth",
                    isSelected ? "isSelected" : "",
                    isToday ? "isToday" : "",
                  ].join(" ")}
                  onClick={() => setSelectedDate(date)}
                  aria-label={`Select ${date.toDateString()}`}
                >
                  <span className="paDay__num">{date.getDate()}</span>

                  {/* Optional dot markers */}
                  {marker === "quiz" && <span className="calDot calDot--quiz" />}
                  {marker === "midterm" && <span className="calDot calDot--midterm" />}
                  {marker === "other" && <span className="calDot calDot--other" />}
                </button>
              );
            })}
          </div>

          {/* Legend */}
          <div className="paLegend">
            {legend.map((it) => (
              <div key={it.label} className="paLegendItem">
                <span className={it.className} aria-hidden="true" />
                <span className="paLegendLabel">{it.label}</span>
              </div>
            ))}
            <div className="paLegendRight">
              <button className="paLinkBtn" type="button">View details</button>
              <button className="paDotsBtn" type="button" aria-label="More options">⋯</button>
            </div>
          </div>

          <div className="paExamMeta">
            <div className="paPill">14:00</div>
            <div className="paPill">A3</div>
          </div>

          <button className="paPrimaryBtn" type="button">
            Schedule
          </button>
        </section>
      </aside>

      {/* MAIN AREA */}
      <main className="paMain">
        {/* Top row: chart + stats cards */}
        <div className="paTopRow">
          {/*Reserved: Grades overview chart */}
          <section className="paCard paChartCard">
            <div className="paCardHeader">
              <h2 className="paTitle">Grades overview</h2>
            </div>

            <div>
              {gradesLoading ? (
                <div className="paChartPlaceholder">
                  <div className="paChartCircle" aria-hidden="true" />
                  <div className="paChartHint">Učitavanje grafikona...</div>
                </div>
              ) : (
                <GradesOverview data={gradeData} size={300} />
              )}
            </div>
          </section>

          {/*Reserved: right stats column */}
          <aside className="paStatsCol">
            <div className="paStatCard">
              <div className="paStatValue">101</div>
              <div className="paStatLabel">ENROLLED</div>
            </div>
            <div className="paStatCard">
              <div className="paStatValue">11</div>
              <div className="paStatLabel">REPEATING</div>
            </div>
            <div className="paStatCard">
              <div className="paStatValue">8.5</div>
              <div className="paStatLabel">AVG GRADE</div>
            </div>
            <div className="paStatCard">
              <div className="paStatValue">81%</div>
              <div className="paStatLabel">PASSED</div>
            </div>
            <div className="paStatCard">
              <div className="paStatValue">90%</div>
              <div className="paStatLabel">AVG ATTENDANCE</div>
            </div>
          </aside>
        </div>

        {/* Bottom row: table */}
        <section className="paCard paTableCard">
          {/*Reserved: table header */}
          <div className="paTableHeader">
            <div className="paTh">Student ID</div>
            <div className="paTh">Attendance</div>
            <div className="paTh">Score</div>
            <div className="paTh">Status</div>
          </div>

          {/*Placeholder rows (remove/replace with real component later) */}
          {[
            { id: "22232", att: "7/10", score: "75/100", ok: true },
            { id: "21232", att: "7/10", score: "23/100", ok: false },
            { id: "22432", att: "7/10", score: "75/100", ok: true },
            { id: "12232", att: "7/10", score: "75/100", ok: true },
            { id: "29232", att: "7/10", score: "75/100", ok: true },
            { id: "25434", att: "7/10", score: "23/100", ok: false },
            { id: "26543", att: "7/10", score: "75/100", ok: true },
            { id: "17654", att: "7/10", score: "23/100", ok: false },
          ].map((r) => (
            <div key={r.id} className="paRow">
              <div className="paTd">{r.id}</div>
              <div className="paTd">{r.att}</div>
              <div className="paTd">{r.score}</div>
              <div className="paTd">
                <span className={r.ok ? "paChip paChip--ok" : "paChip paChip--bad"}>
                  {r.ok ? "✓ Passed" : "✕ Failed"}
                </span>
              </div>
            </div>
          ))}
        </section>
      </main>
    </div>
  );
}
