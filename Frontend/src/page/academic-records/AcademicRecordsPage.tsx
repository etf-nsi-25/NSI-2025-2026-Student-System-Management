import { useEffect, useMemo, useState } from "react";
import dynamic from "next/dynamic";
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
  CAlert,
} from "@coreui/react";

import "@coreui/coreui/dist/css/coreui.min.css";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { useAPI } from "../../context/services";

const Pie = dynamic(
  () =>
    import("react-chartjs-2").then((mod) => {
      ChartJS.register(ArcElement, Tooltip, Legend);
      return mod.Pie;
    }),
  {
    ssr: false,
    loading: () => (
      <div className="text-center p-3">
        <CSpinner size="sm" />
      </div>
    ),
  }
);

type CourseApiDto = {
  id: string;
  name: string;
  code: string;
  ects: number;
  grade?: number;
  examDate?: string;
  professor?: string;
  semester?: number;
};

const formatDate = (iso?: string) => {
  if (!iso) return "-";
  const d = new Date(iso);
  return `${d.getDate()}.${d.getMonth() + 1}.${d.getFullYear()}.`;
};

const stringToColor = (str: string): string => {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash);
  }
  return `hsl(${Math.abs(hash) % 360}, 65%, 55%)`;
};

export default function AcademicRecordsPage() {
  const [isClient, setIsClient] = useState(false);
  const [courses, setCourses] = useState<CourseApiDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedSemester, setSelectedSemester] = useState<number | "all">(
    "all"
  );

  const api = useAPI();

  useEffect(() => {
    setIsClient(true);
  }, []);

  useEffect(() => {
    if (!isClient) return;

    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);

        const data = await api.getAllCourses();
        setCourses(Array.isArray(data) ? (data as unknown as CourseApiDto[]) : []);
      } catch (err: any) {
        setError(err.message || "Error loading academic records from server.");
        setCourses([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [isClient, api]);

  const semesters = useMemo(() => {
    const set = new Set<number>();
    courses.forEach((c) => c.semester && set.add(c.semester));
    return Array.from(set).sort((a, b) => a - b);
  }, [courses]);

  const filteredCourses = useMemo(() => {
    if (selectedSemester === "all") return courses;
    return courses.filter((c) => c.semester === selectedSemester);
  }, [courses, selectedSemester]);

  const completedCourses = useMemo(
    () => filteredCourses.filter((c) => c.grade !== undefined && c.grade >= 6),
    [filteredCourses]
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
        borderWidth: 0,
      },
    ],
  };

  if (!isClient) return null;

  return (
    <div className="p-3 p-md-4 bg-light min-vh-100">
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
        <h2 className="fw-bold mb-0">Academic Records</h2>
        <div className="d-flex gap-2">
          <CButton color="primary" variant="outline" className="shadow-sm">
            Detailed Transcript
          </CButton>
          <CDropdown>
            <CDropdownToggle color="white" className="border shadow-sm">
              {selectedSemester === "all"
                ? "All Semesters"
                : `Semester ${selectedSemester}`}
            </CDropdownToggle>
            <CDropdownMenu>
              <CDropdownItem onClick={() => setSelectedSemester("all")}>
                All Semesters
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

      {error && (
        <CAlert color="danger" className="mb-4">
          {error}
        </CAlert>
      )}

      <CCard className="mb-4 border-0 shadow-sm">
        <CCardBody className="p-0">
          {loading ? (
            <div className="text-center py-5">
              <CSpinner color="primary" />
            </div>
          ) : filteredCourses.length === 0 ? (
            <div className="text-center text-muted py-5">
              No data available.
            </div>
          ) : (
            <CTable hover responsive align="middle" className="mb-0">
              <CTableHead color="light">
                <CTableRow>
                  <CTableHeaderCell className="ps-4">Code</CTableHeaderCell>
                  <CTableHeaderCell>Course Name</CTableHeaderCell>
                  <CTableHeaderCell>ECTS</CTableHeaderCell>
                  <CTableHeaderCell>Grade</CTableHeaderCell>
                  <CTableHeaderCell>Date</CTableHeaderCell>
                  <CTableHeaderCell className="pe-4 text-center">
                    Status
                  </CTableHeaderCell>
                </CTableRow>
              </CTableHead>
              <CTableBody>
                {filteredCourses.map((c) => {
                  const passed = (c.grade ?? 0) >= 6;
                  return (
                    <CTableRow key={c.id}>
                      <CTableDataCell className="ps-4 text-muted small">
                        {c.code}
                      </CTableDataCell>
                      <CTableDataCell className="fw-medium">
                        {c.name}
                      </CTableDataCell>
                      <CTableDataCell>{c.ects}</CTableDataCell>
                      <CTableDataCell className="fw-bold text-primary">
                        {c.grade ?? "-"}
                      </CTableDataCell>
                      <CTableDataCell>{formatDate(c.examDate)}</CTableDataCell>
                      <CTableDataCell className="pe-4 text-center">
                        <span
                          className={`badge ${
                            passed
                              ? "bg-success-subtle text-success"
                              : "bg-danger-subtle text-danger"
                          } px-3 py-2`}
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

      <CRow className="g-4">
        <CCol xs={12} lg={4}>
          <CCard className="h-100 border-0 shadow-sm">
            <CCardBody className="d-flex flex-column align-items-center">
              <h6 className="fw-bold mb-4 align-self-start">ECTS Overview</h6>
              <div style={{ width: "100%", height: "200px" }}>
                <Pie
                  data={semesterPie}
                  options={{ maintainAspectRatio: false }}
                />
              </div>
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={12} lg={4}>
          <CCard className="h-100 border-0 shadow-sm">
            <CCardBody>
              <h6 className="fw-bold mb-4">Grade Distribution</h6>
              {gradeDistribution.map((g, i) => (
                <div key={i} className="mb-3">
                  <div className="d-flex justify-content-between mb-1 small">
                    <span
                      className="text-truncate"
                      style={{ maxWidth: "80%" }}
                    >
                      {g.name}
                    </span>
                    <span className="fw-bold">{g.value / 10}</span>
                  </div>
                  <CProgress
                    value={g.value}
                    height={6}
                    style={{ "--cui-progress-bar-bg": g.color } as any}
                  />
                </div>
              ))}
            </CCardBody>
          </CCard>
        </CCol>

        <CCol xs={12} lg={4}>
          <div className="d-flex flex-column gap-3 h-100">
            <CCard className="border-0 shadow-sm text-center p-4 flex-grow-1 d-flex justify-content-center">
              <div className="text-muted small mb-1">Current GPA</div>
              <div className="display-4 fw-bold text-primary">{gpa}</div>
            </CCard>
            <CCard className="border-0 shadow-sm text-center p-4 flex-grow-1 d-flex justify-content-center bg-primary text-white">
              <div className="small mb-1 opacity-75">ECTS Completed</div>
              <div className="display-4 fw-bold">{completedEcts}</div>
            </CCard>
          </div>
        </CCol>
      </CRow>
    </div>
  );
}