"use client";

import { useEffect, useMemo, useState } from "react";
import {
  CCard,
  CCardBody,
  CCol,
  CRow,
  CTable,
  CTableBody,
  CTableDataCell,
  CTableHead,
  CTableHeaderCell,
  CTableRow,
  CProgress,
  CSpinner,
  CDropdown,
  CDropdownToggle,
  CDropdownMenu,
  CDropdownItem,
  CButton,
} from "@coreui/react";

import "@coreui/coreui/dist/css/coreui.min.css";

import { Pie } from "react-chartjs-2";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";

ChartJS.register(ArcElement, Tooltip, Legend);

/* ===================== TYPES ===================== */

type CourseApiDto = {
  id: string;
  name: string;
  code: string;
  ects: number;
  grade?: number;
  examDate?: string;
  professor?: string;
  semester?: number; // <-- bitno za filter
};

/* ===================== HELPERS ===================== */

const formatDate = (iso?: string) => {
  if (!iso) return "-";
  const d = new Date(iso);
  return `${d.getDate()}.${d.getMonth() + 1}.${d.getFullYear()}`;
};

const stringToColor = (str: string): string => {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash);
  }
  return `hsl(${Math.abs(hash) % 360}, 65%, 55%)`;
};

/* ===================== COMPONENT ===================== */

export default function AcademicRecordsPage() {
  const [courses, setCourses] = useState<CourseApiDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedSemester, setSelectedSemester] = useState<number | "all">(
    "all"
  );

  /* ===================== FETCH ===================== */

  useEffect(() => {
    setLoading(true);

    fetch("https://localhost:5001/api/faculty/courses")
      .then((res) => res.json())
      .then((data) => setCourses(data))
      .catch(() => setCourses([]))
      .finally(() => setLoading(false));
  }, []);

  /* ===================== SEMESTERS ===================== */

  const semesters = useMemo(() => {
    const set = new Set<number>();
    courses.forEach((c) => c.semester && set.add(c.semester));
    return Array.from(set).sort();
  }, [courses]);

  const filteredCourses = useMemo(() => {
    if (selectedSemester === "all") return courses;
    return courses.filter((c) => c.semester === selectedSemester);
  }, [courses, selectedSemester]);

  /* ===================== CALCULATIONS ===================== */

  const completedCourses = filteredCourses.filter(
    (c) => c.grade !== undefined && c.grade >= 6
  );

  const completedEcts = completedCourses.reduce((sum, c) => sum + c.ects, 0);

  const gpa = useMemo(() => {
    if (!completedCourses.length) return "0.00";
    const total = completedCourses.reduce(
      (s, c) => s + (c.grade ?? 0) * c.ects,
      0
    );
    return (total / completedEcts).toFixed(2);
  }, [completedCourses, completedEcts]);

  const gradeDistribution = completedCourses.map((c) => ({
    name: c.name,
    value: (c.grade ?? 0) * 10,
    color: stringToColor(c.name),
  }));

  const semesterPie = {
    labels: ["Completed ECTS", "Remaining"],
    datasets: [
      {
        data: [completedEcts, Math.max(0, 30 - completedEcts)],
        backgroundColor: ["#4f46e5", "#fcd34d"],
      },
    ],
  };

  /* ===================== UI ===================== */

  return (
    <div className="p-3 p-md-4">
      {/* HEADER */}
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
        <h1 className="fw-bold mb-0">Academic Records</h1>

        <div className="d-flex gap-2">
          <CButton color="primary" variant="outline">
            Detailed Transcript
          </CButton>

          <CDropdown>
            <CDropdownToggle color="light">
              {selectedSemester === "all"
                ? "All semesters"
                : `Semester ${selectedSemester}`}
            </CDropdownToggle>
            <CDropdownMenu>
              <CDropdownItem onClick={() => setSelectedSemester("all")}>
                All semesters
              </CDropdownItem>
              {semesters.map((s) => (
                <CDropdownItem key={s} onClick={() => setSelectedSemester(s)}>
                  Semester {s}
                </CDropdownItem>
              ))}
            </CDropdownMenu>
          </CDropdown>
        </div>
      </div>

      {/* COURSE HISTORY */}
      <CCard className="mb-4">
        <CCardBody>
          {loading ? (
            <div className="text-center py-5">
              <CSpinner />
            </div>
          ) : filteredCourses.length === 0 ? (
            <div className="text-center text-muted py-5">
              No courses available.
            </div>
          ) : (
            <CTable hover responsive>
              <CTableHead color="light">
                <CTableRow>
                  <CTableHeaderCell>Code</CTableHeaderCell>
                  <CTableHeaderCell>Name</CTableHeaderCell>
                  <CTableHeaderCell>ECTS</CTableHeaderCell>
                  <CTableHeaderCell>Grade</CTableHeaderCell>
                  <CTableHeaderCell>Date</CTableHeaderCell>
                  <CTableHeaderCell>Status</CTableHeaderCell>
                </CTableRow>
              </CTableHead>

              <CTableBody>
                {filteredCourses.map((c) => {
                  const passed = c.grade !== undefined && c.grade >= 6;
                  return (
                    <CTableRow key={c.id}>
                      <CTableDataCell>{c.code}</CTableDataCell>
                      <CTableDataCell>{c.name}</CTableDataCell>
                      <CTableDataCell>{c.ects}</CTableDataCell>
                      <CTableDataCell>{c.grade ?? "-"}</CTableDataCell>
                      <CTableDataCell>{formatDate(c.examDate)}</CTableDataCell>
                      <CTableDataCell>
                        <span
                          className={`px-2 py-1 rounded text-white ${
                            passed ? "bg-success" : "bg-danger"
                          }`}
                        >
                          {passed ? "Pass" : "Fail"}
                        </span>
                      </CTableDataCell>
                    </CTableRow>
                  );
                })}
              </CTableBody>
            </CTable>
          )}
        </CCardBody>
      </CCard>

      {/* STATS */}
      <CRow className="g-4">
        <CCol xs={12} md={4}>
          <CCard className="h-100">
            <CCardBody>
              <h6 className="fw-semibold mb-3">Semester Overview</h6>
              <Pie data={semesterPie} />
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={12} md={4}>
          <CCard className="h-100">
            <CCardBody>
              <h6 className="fw-semibold mb-3">Grade Distribution</h6>
              {gradeDistribution.map((g, i) => (
                <div key={i} className="mb-3">
                  <div className="d-flex justify-content-between mb-1">
                    <small>{g.name}</small>
                    <small>{g.value}%</small>
                  </div>
                  <CProgress
                    value={g.value}
                    height={8}
                    style={
                      {
                        "--cui-progress-bar-bg": g.color,
                      } as React.CSSProperties
                    }
                  />
                </div>
              ))}
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={12} md={4}>
          <CCard className="mb-3">
            <CCardBody className="text-center">
              <div className="fw-semibold">Current GPA</div>
              <div className="fs-2 fw-bold text-primary">{gpa}</div>
            </CCardBody>
          </CCard>

          <CCard>
            <CCardBody className="text-center">
              <div className="fw-semibold">ECTS Completed</div>
              <div className="fs-2 fw-bold text-primary">{completedEcts}</div>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </div>
  );
}
