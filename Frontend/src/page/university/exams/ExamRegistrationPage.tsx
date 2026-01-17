import { useCallback, useEffect, useState } from "react";
import { CFormInput, CRow, CCol } from "@coreui/react";
import { ExamCard } from "../../../component/exams/ExamCard";
import { ExamEmptyState } from "../../../component/exams/ExamEmptyState";
import { useToast } from "../../../context/toast";
import { extractApiErrorMessage } from "../../../utils/apiError";
import { useAPI } from "../../../context/services";
import "./ExamRegistrationPage.css";
import type {
  AvailableStudentExamDto,
  RegisteredStudentExamDto,
} from "../../../dto/StudentExamsDTO";

const delay = (ms: number) =>
  new Promise(resolve => setTimeout(resolve, ms));


type ExamItem = {
  id: number;
  courseName: string;
  courseCode: string;
  examDate: string;
  regDeadline?: string;
  location?: string;
  registrationDate?: string;
};

function mapAvailableExam(dto: AvailableStudentExamDto): ExamItem | null {
  if (!dto.examDate || !dto.registrationDeadline) return null;

  return {
    id: dto.examId,
    courseName: dto.courseName,
    courseCode: dto.courseCode,
    examDate: dto.examDate,
    regDeadline: dto.registrationDeadline,
  };
}

function mapRegisteredExam(dto: RegisteredStudentExamDto): ExamItem | null {
  if (!dto.examDate) return null;

  return {
    id: dto.examId,
    courseName: dto.courseName,
    courseCode: dto.courseCode,
    examDate: dto.examDate,
    registrationDate: dto.registrationDate,
  };
}

export default function ExamRegistrationPage() {
  const api = useAPI();
  const { pushToast } = useToast();
  const [success, setSuccess] = useState<string | null>(null);
  const [tab, setTab] = useState<"available" | "registered">("available");
  const [search, setSearch] = useState("");
  const [loadingId, setLoadingId] = useState<number | null>(null);
  const [available, setAvailable] = useState<ExamItem[] | null>(null);
  const [registered, setRegistered] = useState<ExamItem[] | null>(null);

  const loadData = useCallback(async () => {
    try {
      const [availableDtos, registeredDtos] = await Promise.all([
        api.getAvailableStudentExams(),
        api.getRegisteredStudentExams(),
      ]);

      setAvailable(
        availableDtos
          .map(mapAvailableExam)
          .filter((x): x is ExamItem => x !== null)
      );

      setRegistered(
        registeredDtos
          .map(mapRegisteredExam)
          .filter((x): x is ExamItem => x !== null)
      );
    } catch (e: any) {
      const status = e?.status as number | undefined;
      if (!status || status >= 500) {
        pushToast("error", "Something went wrong", "Please try again later.");
      } else {
        const msg = extractApiErrorMessage(e);
        pushToast("error", "Failed to load exams", msg);
      }

      setAvailable([]);
      setRegistered([]);
    }
  }, [api, pushToast]);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  const now = new Date();

  // AVAILABLE – search + filter future + sort
  const availableFiltered = (available ?? [])
    .filter((exam) =>
      exam.courseName.toLowerCase().includes(search.toLowerCase())
    )
    .filter((exam) => new Date(exam.examDate) > now) // hide past
    .sort(
      (a, b) =>
        new Date(a.examDate).getTime() - new Date(b.examDate).getTime()
    );

  // REGISTERED – search + sort
  const registeredFiltered = (registered ?? [])
    .filter((exam) =>
      exam.courseName.toLowerCase().includes(search.toLowerCase())
    )
    .sort(
      (a, b) =>
        new Date(a.examDate).getTime() - new Date(b.examDate).getTime()
    );

  const handleRegister = async (id: number) => {
    setSuccess(null);
    setLoadingId(id);

    try {
      const exam = (available ?? []).find(x => x.id === id);
      if (!exam) return;

      if (!exam.regDeadline || new Date(exam.regDeadline) < new Date()) {
        pushToast("error", "Cannot register", "Registration deadline has passed.");
        return;
      }

      await Promise.all([
        api.registerForStudentExam(id),
        delay(600), // UX delay – loading must be visible
      ]);

      await loadData();

      setSuccess("Successfully registered for the exam.");
    } catch (e: any) {
      const status = e?.status as number | undefined;
      if (!status || status >= 500) {
        pushToast("error", "Something went wrong", "Please try again later.");
      } else {
        const msg = extractApiErrorMessage(e);
        pushToast("error", "Registration failed", msg);
      }
    } finally {
      setLoadingId(null);
    }
  };


  return (
    <div className="exam-registration-container">
      <div className="exam-registration-content">
        {/* TITLE */}
        <h1 className="exam-page-title">Exam Registration</h1>

        {/* SEARCH */}
        <div className="exam-search-wrapper">
          <CFormInput
            className="exam-search-input"
            placeholder="Search for a course..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        {/* TABS */}
        <div className="exam-tabs-wrapper">
          <button
            className={`exam-tab-btn ${tab === "available" ? "active" : ""}`}
            onClick={() => {
              setTab("available");
              setSuccess(null);
            }}
          >
            Available Exams
          </button>

          <button
            className={`exam-tab-btn ${tab === "registered" ? "active" : ""}`}
            onClick={() => {
              setTab("registered");
              setSuccess(null);
            }}
          >
            Registered Exams
          </button>
        </div>

        <div className="exam-info-banner-wrapper">
          <div className="exam-info-banner">
            {tab === "available" ? (
              <>
                Showing exams you are eligible to register for. Only upcoming
                exams are listed, already sorted by date.
              </>
            ) : (
              <>
                These are exams you&apos;re registered for. Sorted by exam date.
              </>
            )}
          </div>
        </div>

        {success && (
          <div className="d-flex justify-content-center mb-4">
            <div className="ui-alert ui-alert-success w-100" style={{ maxWidth: 800 }}>
              {success}
            </div>
          </div>
        )}


        {/* AVAILABLE TAB */}
        {tab === "available" && (
          <>
            {available !== null && availableFiltered.length === 0 ? (
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
            {registered !== null && registeredFiltered.length === 0 ? (
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
