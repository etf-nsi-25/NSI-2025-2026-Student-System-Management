import { useState } from "react";
import { CFormInput, CRow, CCol } from "@coreui/react";
import { ExamCard } from "../../../component/exams/ExamCard";
import { examService } from "../../../service/examService";
import { ExamEmptyState } from "../../../component/exams/ExamEmptyState";

const delay = (ms: number) =>
  new Promise(resolve => setTimeout(resolve, ms));


type ExamItem = {
  id: number;
  courseName: string;
  courseCode: string;
  examDate: string;
  regDeadline: string;
  location: string;
};

// MOCK – Available
const mockAvailable: ExamItem[] = [
  {
    id: 1,
    courseName: "Algorithms and Data Structures",
    courseCode: "ADS101",
    examDate: "2026-02-10T09:00:00Z",
    regDeadline: "2026-02-06T23:59:00Z",
    location: "Room 301 - ETF",
  },
  {
    id: 2,
    courseName: "Computer Networks",
    courseCode: "CN201",
    examDate: "2026-02-12T11:00:00Z",
    regDeadline: "2026-02-07T23:59:00Z",
    location: "Lab 2 - ETF",
  },
  {
    id: 3,
    courseName: "Operating Systems",
    courseCode: "OS150",
    examDate: "2026-02-14T08:00:00Z",
    regDeadline: "2026-02-09T23:59:00Z",
    location: "Room 207 - ETF",
  },
  {
    id: 4,
    courseName: "Database Systems",
    courseCode: "DB265",
    examDate: "2026-02-18T12:00:00Z",
    regDeadline: "2026-02-11T23:59:00Z",
    location: "Room 105 - ETF",
  },
  {
    id: 5,
    courseName: "Software Engineering",
    courseCode: "SE310",
    examDate: "2026-02-20T10:00:00Z",
    regDeadline: "2026-02-13T23:59:00Z",
    location: "Room 220 - ETF",
  },
  {
    id: 6,
    courseName: "Machine Learning Basics",
    courseCode: "ML101",
    examDate: "2026-02-22T13:00:00Z",
    regDeadline: "2026-02-16T23:59:00Z",
    location: "Room 112 - ETF",
  },
  {
    id: 7,
    courseName: "Digital Logic Design",
    courseCode: "DLD140",
    examDate: "2026-02-25T09:00:00Z",
    regDeadline: "2026-02-18T23:59:00Z",
    location: "Room 108 - ETF",
  },
  {
    id: 8,
    courseName: "Discrete Mathematics",
    courseCode: "DM120",
    examDate: "2026-02-27T11:00:00Z",
    regDeadline: "2026-02-21T23:59:00Z",
    location: "Room 115 - ETF",
  },
];

// MOCK – Registered
const mockRegistered: ExamItem[] = [
  {
    id: 101,
    courseName: "Software Engineering",
    courseCode: "SE310",
    examDate: "2025-02-20T10:00:00Z",
    regDeadline: "2025-02-13T23:59:00Z",
    location: "Room 220 - ETF",
  },
  {
    id: 102,
    courseName: "Database Systems",
    courseCode: "DB265",
    examDate: "2025-02-18T12:00:00Z",
    regDeadline: "2025-02-11T23:59:00Z",
    location: "Room 105 - ETF",
  },
  {
    id: 103,
    courseName: "Computer Architecture",
    courseCode: "CA210",
    examDate: "2025-03-01T09:00:00Z",
    regDeadline: "2025-02-23T23:59:00Z",
    location: "Room 204 - ETF",
  },
  {
    id: 104,
    courseName: "Web Development",
    courseCode: "WD150",
    examDate: "2025-03-02T14:00:00Z",
    regDeadline: "2025-02-24T23:59:00Z",
    location: "Lab 3 - ETF",
  },
  {
    id: 105,
    courseName: "Advanced Algorithms",
    courseCode: "AA330",
    examDate: "2025-03-04T11:00:00Z",
    regDeadline: "2025-02-26T23:59:00Z",
    location: "Room 302 - ETF",
  },
  {
    id: 106,
    courseName: "Linear Algebra",
    courseCode: "LA110",
    examDate: "2025-03-05T09:00:00Z",
    regDeadline: "2025-02-27T23:59:00Z",
    location: "Room 101 - ETF",
  },
  {
    id: 107,
    courseName: "Numerical Methods",
    courseCode: "NM250",
    examDate: "2025-03-06T13:00:00Z",
    regDeadline: "2025-02-28T23:59:00Z",
    location: "Room 218 - ETF",
  },
  {
    id: 108,
    courseName: "Applied Statistics",
    courseCode: "AS215",
    examDate: "2025-03-07T12:00:00Z",
    regDeadline: "2025-03-01T23:59:00Z",
    location: "Room 107 - ETF",
  },
  {
    id: 109,
    courseName: "Object-Oriented Programming",
    courseCode: "OOP200",
    examDate: "2025-03-09T10:00:00Z",
    regDeadline: "2025-03-02T23:59:00Z",
    location: "Room 206 - ETF",
  },
  {
    id: 110,
    courseName: "Signals and Systems",
    courseCode: "SS220",
    examDate: "2025-03-11T11:00:00Z",
    regDeadline: "2025-03-03T23:59:00Z",
    location: "Room 210 - ETF",
  },
  {
    id: 111,
    courseName: "Data Mining",
    courseCode: "DM450",
    examDate: "2025-03-13T14:00:00Z",
    regDeadline: "2025-03-05T23:59:00Z",
    location: "Room 115 - ETF",
  },
  {
    id: 112,
    courseName: "Parallel Computing",
    courseCode: "PC480",
    examDate: "2025-03-15T09:00:00Z",
    regDeadline: "2025-03-07T23:59:00Z",
    location: "Room 303 - ETF",
  },
];

export default function ExamRegistrationPage() {

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [tab, setTab] = useState<"available" | "registered">("available");
  const [search, setSearch] = useState("");
  const [loadingId, setLoadingId] = useState<number | null>(null);
  const [available, setAvailable] = useState<ExamItem[]>(mockAvailable);
  const [registered, setRegistered] = useState<ExamItem[]>(mockRegistered);

  const now = new Date();

  // AVAILABLE – search + filter future + sort
  const availableFiltered = available
    .filter((exam) =>
      exam.courseName.toLowerCase().includes(search.toLowerCase())
    )
    .filter((exam) => new Date(exam.examDate) > now) // hide past
    .sort(
      (a, b) =>
        new Date(a.examDate).getTime() - new Date(b.examDate).getTime()
    );

  // REGISTERED – search + sort
  const registeredFiltered = registered
    .filter((exam) =>
      exam.courseName.toLowerCase().includes(search.toLowerCase())
    )
    .sort(
      (a, b) =>
        new Date(a.examDate).getTime() - new Date(b.examDate).getTime()
    );

    const handleRegister = async (id: number) => {
      setError(null);
      setSuccess(null);
      setLoadingId(id);
    
      try {
        const exam = available.find(x => x.id === id);
        if (!exam) return;
    
        if (new Date(exam.regDeadline) < new Date()) {
          setError("Registration deadline has passed.");
          return;
        }
    
        await Promise.all([
          examService.registerForExam(id),
          delay(600), // UX delay – loading must be visible
        ]);
    
        setRegistered(prev => [
          ...prev,
          { ...exam, registrationDate: new Date().toISOString() },
        ]);
    
        setAvailable(prev => prev.filter(x => x.id !== id));
    
        setSuccess("Successfully registered for the exam.");
      } catch (e: any) {
        if (e?.status === 400) {
          setError("You are already registered for this exam.");
        } else if (e?.status === 401 || e?.status === 403) {
          setError("You are not authorized to perform this action.");
        } else {
          setError("Something went wrong. Please try again later.");
        }
      } finally {
        setLoadingId(null);
      }
    };
    
    const handleUnregister = async (id: number) => {
      setError(null);
      setSuccess(null);
      setLoadingId(id);
    
      try {
        const exam = registered.find(x => x.id === id);
        if (!exam) return;
    
        await Promise.all([
          examService.unregisterExam(id),
          delay(600), // UX delay – loading must be visible
        ]);
    
        // vraćamo u available SAMO ako deadline nije prošao
        if (new Date(exam.regDeadline) > new Date()) {
          setAvailable(prev => [...prev, exam]);
        }
    
        setRegistered(prev => prev.filter(x => x.id !== id));
    
        setSuccess("Successfully unregistered from the exam.");
      } catch {
        setError("Unable to unregister from the exam.");
      } finally {
        setLoadingId(null);
      }
    };
    


  return (
    <div
      style={{
        width: "100%",
        minHeight: "100vh",
        padding: "40px 16px",
        background: "radial-gradient(circle at top, #151521, #050509)",
        color: "#fff",
      }}
    >
      <div style={{ width: "100%", maxWidth: 1200, margin: "0 auto" }}>
        {/* TITLE */}
        <h1 className="ui-heading-lg mb-4 text-center">Exam Registration</h1>

        {/* SEARCH */}
        <div className="d-flex justify-content-center mb-4">
          <CFormInput
            className="ui-input-base"
            placeholder="Search for a course..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            style={{ maxWidth: 480 }}
          />
        </div>

        {/* TABS */}
        <div className="d-flex justify-content-center mb-3 gap-3">
          <button
            className={`ui-tab-btn ${tab === "available" ? "active" : ""}`}
            onClick={() => {
              setTab("available");
              setError(null);
              setSuccess(null);
            }}
          >
            Available Exams
          </button>

          <button
            className={`ui-tab-btn ${tab === "registered" ? "active" : ""}`}
            onClick={() => {
              setTab("registered");
              setError(null);
              setSuccess(null);
            }}
          >
            Registered Exams
          </button>

        </div>

        {/* INFO PILL – objašnjenje šta je već primijenjeno */}
        <div className="d-flex justify-content-center mb-5">
          <div className="ui-info-banner">
            {tab === "available" ? (
              <>
                Showing exams you are eligible to register for. Only upcoming
                exams are listed, already sorted by date. Use search to filter
                by course name.
              </>
            ) : (
              <>
                These are exams you&apos;re registered for. Sorted by exam date.
                You can unregister before the registration deadline.
              </>
            )}
          </div>
        </div>
        {error && (
          <div className="d-flex justify-content-center mb-3">
            <div className="ui-alert ui-alert-error">{error}</div>
          </div>
        )}

        {success && (
          <div className="d-flex justify-content-center mb-3">
            <div className="ui-alert ui-alert-success">{success}</div>
          </div>
        )}


        {/* AVAILABLE TAB */}
        {tab === "available" && (
          <>
            {availableFiltered.length === 0 ? (
              <ExamEmptyState
                title="No available exams"
                description="There are currently no exams you can register for."
              />
            ) : (
              <CRow className="g-4">
                {availableFiltered.map((exam) => (
                  <CCol xs={12} md={6} lg={4} key={exam.id}>
                    <ExamCard
                      exam={exam}
                      isRegistered={false}
                      loading={loadingId === exam.id}
                      onRegister={handleRegister}
                    />
                  </CCol>
                ))}
              </CRow>
            )}
          </>
        )}

        {/* REGISTERED TAB */}
        {tab === "registered" && (
          <>
            {registeredFiltered.length === 0 ? (
              <ExamEmptyState
                title="No registered exams"
                description="You have not registered for any exams yet."
              />

            ) : (
              <CRow className="g-4">
                {registeredFiltered.map((exam) => (
                  <CCol xs={12} md={6} lg={4} key={exam.id}>
                    <ExamCard
                      exam={exam}
                      isRegistered
                      loading={loadingId === exam.id}
                      onUnregister={handleUnregister}
                    />
                  </CCol>
                ))}
              </CRow>
            )}
          </>
        )}
      </div>
    </div>
  );
}
