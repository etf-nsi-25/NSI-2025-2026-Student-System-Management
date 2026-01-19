import { useEffect, useState } from "react";
import {
    CCard,
    CCardBody,
    CCardText,
    CCardTitle,
    CCol,
    CRow,
    CSpinner,
    CAlert,
    CButton,
    CFormInput,
    CInputGroup,
    CInputGroupText,
    CListGroup,
    CListGroupItem,
} from "@coreui/react";
import {
  Search,
  ArrowRight,
  Calendar as CalendarIcon,
} from "lucide-react";
import {
  format,
  startOfMonth,
  endOfMonth,
  eachDayOfInterval,
  isToday,
} from "date-fns";
import { getDashboardData } from "../../service/dashboard/api";
import { useAPI } from "../../context/services";
import {
    getMyEnrollments,
    getTeacherForCourse,
    type StudentEnrollmentItemDto,
    type TeacherDto,
} from "../../service/enrollment/api";

import "./dashboard.css";

interface DashboardData {
    user: {
        name: string;
        faculty: string;
    };
    stats: {
        gpa: number;
        enrolledCourses: number;
        attendanceRate: number;
        deadlines: {
            count: number;
            period: string;
        };
    };
    courses: Array<{
        id: string;
        title: string;
        professor: string;
    }>;
    upcomingTasks: Array<{
        id: number;
        course: string;
        task: string;
        day: string;
    }>;
    calendar: {
        currentMonth: string;
        highlightedDates: number[];
    };
    quickActions: Array<{
        id: number;
        label: string;
        icon: string;
        color: string;
    }>;
}

type DemoDashboardCourse = {
    id: number | string;
    title: string;
    professor: string;
};

export default function DashboardPage() {
    const api = useAPI();

    const [data, setData] = useState<DashboardData | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [enrollmentsError, setEnrollmentsError] = useState<string | null>(null);

    const [currentDate] = useState(new Date(2025, 10, 1));

    useEffect(() => {
        async function fetchDashboardData() {
            try {
                setLoading(true);
                setError(null);
                setEnrollmentsError(null);

                const result = await getDashboardData();

                const normalizedResult: DashboardData = {
                    ...result,
                    courses: result.courses.map((c: DemoDashboardCourse) => ({
                        ...c,
                        id: String(c.id),
                    })),
                };

                try {
                    const enrollments = (await getMyEnrollments(api)) as StudentEnrollmentItemDto[];

                    const teacherNames = await Promise.all(
                        enrollments.map(async (e) => {
                            try {
                                const t = (await getTeacherForCourse(api, e.courseId)) as TeacherDto;
                                return t?.fullName ?? "—";
                            } catch {
                                return "—";
                            }
                        }),
                    );

                    const mappedCourses = enrollments.map((e, idx) => ({
                        id: e.courseId,
                        title: e.courseName,
                        professor: teacherNames[idx] ?? "—",
                    }));

                    setData({
                        ...normalizedResult,
                        courses: mappedCourses,
                        stats: {
                            ...normalizedResult.stats,
                            enrolledCourses: enrollments.length,
                        },
                    });
                } catch (e) {
                    console.error("Failed to load enrollments:", e);
                    setEnrollmentsError("Could not load your enrollments (showing demo courses).");
                    setData(normalizedResult);
                }
            } catch (err) {
                setError(err instanceof Error ? err.message : "An error occurred");
            } finally {
                setLoading(false);
            }
        }

        fetchDashboardData();
    }, [api]);

    const monthStart = startOfMonth(currentDate);
    const monthEnd = endOfMonth(currentDate);
    const daysInMonth = eachDayOfInterval({ start: monthStart, end: monthEnd });
    const firstDayOfWeek = monthStart.getDay();
    const calendarDays = Array(firstDayOfWeek).fill(null).concat(daysInMonth);

    const getDateColor = (date: Date | null) => {
        if (!date || !data) return "";
        const day = date.getDate();

        if (data.calendar.highlightedDates.includes(day)) {
            const colors = ["#a78bfa", "#60a5fa", "#34d399"];
            const index = data.calendar.highlightedDates.indexOf(day) % colors.length;
            return colors[index];
        }
        return "";
    };

    if (loading) {
        return (
            <div className="dashboard-loading">
                <CSpinner color="primary" />
            </div>
        );
    }

    if (error) {
        return (
            <div className="dashboard-page">
                <CAlert color="danger">
                    <h4 className="alert-heading">Error loading dashboard</h4>
                    <p>{error}</p>
                </CAlert>
            </div>
        );
    }

    if (!data) return null;

    return (
        <div className="dashboard-page">
            <CRow className="mb-4 align-items-center">
                <CCol xs={12} lg={8}>
                    <h1 className="mb-3 dashboard-header-title">Welcome back, {data.user.name}!</h1>
                </CCol>
                <CCol xs={12} lg={4}>
                    <CInputGroup className="dashboard-search-input">
                        <CFormInput placeholder="Search courses, grades ..." />
                        <CInputGroupText className="dashboard-search-icon">
                            <Search size={20} />
                        </CInputGroupText>
                    </CInputGroup>
                </CCol>
            </CRow>

            <CRow className="mb-4 g-3">
                <CCol xs={6} md={3}>
                    <CCard className="dashboard-stats-card">
                        <CCardBody className="text-center">
                            <CCardTitle className="dashboard-stats-value">{data.stats.gpa}</CCardTitle>
                            <CCardText className="dashboard-stats-label">GAP</CCardText>
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol xs={6} md={3}>
                    <CCard className="dashboard-stats-card">
                        <CCardBody className="text-center">
                            <CCardTitle className="dashboard-stats-value">{data.stats.enrolledCourses}</CCardTitle>
                            <CCardText className="dashboard-stats-label">Enrolled courses</CCardText>
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol xs={6} md={3}>
                    <CCard className="dashboard-stats-card">
                        <CCardBody className="text-center">
                            <CCardTitle className="dashboard-stats-value">{data.stats.attendanceRate}%</CCardTitle>
                            <CCardText className="dashboard-stats-label">Attendance rate</CCardText>
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol xs={6} md={3}>
                    <CCard className="dashboard-stats-card">
                        <CCardBody className="text-center">
                            <CCardTitle className="dashboard-stats-value">
                                {data.stats.deadlines.count} deadlines
                            </CCardTitle>
                            <CCardText className="dashboard-stats-label">{data.stats.deadlines.period}</CCardText>
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            <CRow className="g-3">
                <CCol xs={12} lg={8}>
                    <h2 className="mb-3 dashboard-section-title">My courses</h2>

                    {enrollmentsError && (
                        <CAlert color="warning" className="mb-3">
                            {enrollmentsError}
                        </CAlert>
                    )}

                    <CRow className="mb-4 g-3">
                        {data.courses.map((course, idx) => (
                            <CCol key={`${course.id}-${idx}`} xs={12} md={6} lg={4}>
                                <CCard className="dashboard-course-card h-100">
                                    <CCardBody className="d-flex flex-column justify-content-between">
                                        <div>
                                            <div className="d-flex justify-content-end mb-2">
                                                <ArrowRight size={20} />
                                            </div>
                                            <CCardTitle className="dashboard-course-title">{course.title}</CCardTitle>
                                        </div>
                                        <CCardText className="dashboard-course-professor">{course.professor}</CCardText>
                                    </CCardBody>
                                </CCard>
                            </CCol>
                        ))}
                    </CRow>

                    <h2 className="mb-3 dashboard-section-title">Upcoming this week</h2>
                    <CListGroup>
                        {data.upcomingTasks.map((task) => (
                            <CListGroupItem key={task.id}>
                                <div className="d-flex justify-content-between align-items-center">
                                    <div className="d-flex gap-3">
                                        <CalendarIcon size={20} />
                                        <div>
                                            <div>
                                                {task.course} - {task.task}
                                            </div>
                                            <div>{task.day}</div>
                                        </div>
                                    </div>
                                    <CButton size="sm">Submit</CButton>
                                </div>
                            </CListGroupItem>
                        ))}
                    </CListGroup>
                </CCol>

                <CCol xs={12} lg={4}>
                    <CCard>
                        <CCardBody>
                            <h3 className="text-center mb-3">{format(currentDate, "MMMM yyyy")}</h3>

                            <div className="dashboard-calendar-grid">
                                {["S", "M", "T", "W", "T", "F", "S"].map((d) => (
                                    <div key={d}>{d}</div>
                                ))}
                                {calendarDays.map((date, idx) => {
                                    const bg = getDateColor(date);
                                    return (
                                        <div
                                            key={idx}
                                            style={{
                                                backgroundColor: bg || "transparent",
                                                fontWeight: bg || (date && isToday(date)) ? 600 : 400,
                                            }}
                                        >
                                            {date ? format(date, "d") : ""}
                                        </div>
                                    );
                                })}
                            </div>
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>
        </div>
    );
}
