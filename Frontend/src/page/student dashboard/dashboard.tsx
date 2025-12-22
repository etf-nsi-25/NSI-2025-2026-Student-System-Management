"use client";

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
  FileText,
  Award,
  MoreHorizontal,
} from "lucide-react";
import {
  format,
  startOfMonth,
  endOfMonth,
  eachDayOfInterval,
  isToday,
} from "date-fns";
import { getDashboardData } from "../../service/dashboard/api";

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
    id: number;
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

export default function DashboardPage() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentDate] = useState(new Date(2025, 10, 1)); // November 2025

  useEffect(() => {
    async function fetchDashboardData() {
      try {
        setLoading(true);
        const result = await getDashboardData();
        setData(result);
      } catch (err) {
        setError(err instanceof Error ? err.message : "An error occurred");
      } finally {
        setLoading(false);
      }
    }

    fetchDashboardData();
  }, []);

  // Generate calendar days
  const monthStart = startOfMonth(currentDate);
  const monthEnd = endOfMonth(currentDate);
  const daysInMonth = eachDayOfInterval({ start: monthStart, end: monthEnd });

  // Get day of week for first day (0 = Sunday, 1 = Monday, etc.)
  const firstDayOfWeek = monthStart.getDay();

  // Generate array to include empty cells for days before month starts
  const calendarDays = Array(firstDayOfWeek).fill(null).concat(daysInMonth);

  const getDateColor = (date: Date | null) => {
    if (!date || !data) return "";
    const day = date.getDate();

    if (data.calendar.highlightedDates.includes(day)) {
      // Cycle through colors for different dates
      const colors = ["#a78bfa", "#60a5fa", "#34d399"]; // purple, blue, green
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

  if (!data) {
    return null;
  }

  return (
    <div className="dashboard-page">
      {/* Header */}
      <CRow className="mb-4 align-items-center">
        <CCol xs={12} lg={8}>
          <h1 className="mb-3 dashboard-header-title">
            Welcome back, {data.user.name}!
          </h1>
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

      {/* Stats Cards */}
      <CRow className="mb-4 g-3">
        <CCol xs={6} md={3}>
          <CCard className="dashboard-stats-card">
            <CCardBody className="text-center">
              <CCardTitle className="dashboard-stats-value">
                {data.stats.gpa}
              </CCardTitle>
              <CCardText className="dashboard-stats-label">GAP</CCardText>
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={6} md={3}>
          <CCard className="dashboard-stats-card">
            <CCardBody className="text-center">
              <CCardTitle className="dashboard-stats-value">
                {data.stats.enrolledCourses}
              </CCardTitle>
              <CCardText className="dashboard-stats-label">
                Enrolled courses
              </CCardText>
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={6} md={3}>
          <CCard className="dashboard-stats-card">
            <CCardBody className="text-center">
              <CCardTitle className="dashboard-stats-value">
                {data.stats.attendanceRate}%
              </CCardTitle>
              <CCardText className="dashboard-stats-label">
                Attendance rate
              </CCardText>
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={6} md={3}>
          <CCard className="dashboard-stats-card">
            <CCardBody className="text-center">
              <CCardTitle className="dashboard-stats-value">
                {data.stats.deadlines.count} deadlines
              </CCardTitle>
              <CCardText className="dashboard-stats-label">
                {data.stats.deadlines.period}
              </CCardText>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>

      <CRow className="g-3">
        {/* LEFT SIDE */}
        <CCol xs={12} lg={8}>
          {/* My Courses */}
          <h2 className="mb-3 dashboard-section-title">My courses</h2>
          <CRow className="mb-4 g-3">
            {data.courses.map((course) => (
              <CCol key={course.id} xs={12} md={6} lg={4}>
                <CCard className="dashboard-course-card h-100">
                  <CCardBody className="d-flex flex-column justify-content-between">
                    <div>
                      <div className="d-flex justify-content-end mb-2">
                        <ArrowRight size={20} />
                      </div>
                      <CCardTitle className="dashboard-course-title">
                        {course.title}
                      </CCardTitle>
                    </div>
                    <CCardText className="dashboard-course-professor">
                      {course.professor}
                    </CCardText>
                  </CCardBody>
                </CCard>
              </CCol>
            ))}
          </CRow>

          {/* Upcoming this week */}
          <h2 className="mb-3 dashboard-section-title">Upcoming this week</h2>
          <CListGroup>
            {data.upcomingTasks.map((task) => (
              <CListGroupItem
                key={task.id}
                className="dashboard-upcoming-item"
              >
                <div className="d-flex justify-content-between align-items-center">
                  <div className="d-flex align-items-start gap-3">
                    <CalendarIcon
                      size={20}
                      className="dashboard-upcoming-icon"
                    />
                    <div>
                      <div className="dashboard-upcoming-title">
                        {task.course} - {task.task}
                      </div>
                      <div className="dashboard-upcoming-day">
                        {task.day}
                      </div>
                    </div>
                  </div>
                  <CButton
                    color="primary"
                    size="sm"
                    className="dashboard-upcoming-button"
                  >
                    Submit
                  </CButton>
                </div>
              </CListGroupItem>
            ))}
          </CListGroup>
        </CCol>

        {/* RIGHT SIDE */}
        <CCol xs={12} lg={4} className="dashboard-right-col">
          <CCard className="dashboard-right-card">
            <CCardBody>
              {/* Calendar */}
              <div className="mb-4">
                <div className="d-flex justify-content-center mb-3">
                  <h3 className="dashboard-calendar-title">
                    {format(currentDate, "MMMM yyyy")}
                  </h3>
                </div>

                <div className="dashboard-calendar-grid">
                  {/* Weekday headers */}
                  {["S", "M", "T", "W", "T", "F", "S"].map((day, idx) => (
                    <div
                      key={idx}
                      className="dashboard-calendar-weekday"
                    >
                      {day}
                    </div>
                  ))}

                  {/* Calendar days */}
                  {calendarDays.map((date, idx) => {
                    const bgColor = date ? getDateColor(date) : "";
                    const textColor = bgColor ? "white" : "#1f2937";
                    const isCurrentDay = date && isToday(date);

                    return (
                      <div
                        key={idx}
                        className="dashboard-calendar-day"
                        style={{
                          backgroundColor: bgColor || "transparent",
                          color: date ? textColor : "transparent",
                          fontWeight: bgColor || isCurrentDay ? 600 : 400,
                          border: isCurrentDay ? "2px solid #3b82f6" : "none",
                        }}
                      >
                        {date ? format(date, "d") : ""}
                      </div>
                    );
                  })}
                </div>
              </div>

              {/* Quick Actions */}
              <div>
                <h3 className="mb-3 dashboard-quick-title">Quick Actions</h3>
                <div className="d-flex flex-column gap-2">
                  {data.quickActions.map((action) => {
                    const iconMap: Record<string, any> = {
                      assignment: FileText,
                      document: FileText,
                      certificate: Award,
                    };
                    const Icon = iconMap[action.icon] || FileText;

                    const colorMap: Record<string, string> = {
                      purple: "#a78bfa",
                      blue: "#60a5fa",
                      green: "#34d399",
                    };

                    return (
                      <div
                        key={action.id}
                        className="dashboard-quick-action"
                      >
                        <div className="d-flex align-items-center gap-3">
                          <div
                            className="dashboard-quick-action-icon"
                            style={{
                              backgroundColor: colorMap[action.color],
                            }}
                          >
                            <Icon size={20} />
                          </div>
                          <span className="dashboard-quick-action-label">
                            {action.label}
                          </span>
                        </div>
                        <MoreHorizontal
                          size={20}
                          className="dashboard-quick-action-more"
                        />
                      </div>
                    );
                  })}
                </div>
              </div>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </div>
  );
}
