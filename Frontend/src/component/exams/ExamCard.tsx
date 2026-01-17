// src/component/exams/ExamCard.tsx
import { CCard, CCardBody, CBadge, CButton, CSpinner } from "@coreui/react";
import CIcon from "@coreui/icons-react";
import {
  cilLocationPin,
  cilCalendar,
  cilAlarm,
  cilCheckCircle,
} from "@coreui/icons";
import { formatDateTime, formatDateOnly } from "../../utils/dateFormatting";

type ExamCardProps = {
  exam: {
    id: number;
    courseName: string;
    courseCode: string;
    examDate: string;
    regDeadline?: string;
    location?: string;
    registrationDate?: string;
  };
  isRegistered?: boolean;
  loading?: boolean;
  onRegister?: (id: number) => void;
};

export function ExamCard({
  exam,
  isRegistered = false,
  loading = false,
  onRegister,
}: ExamCardProps) {
  const examDate = formatDateTime(exam.examDate);
  const regDeadlineText = exam.regDeadline
    ? formatDateOnly(exam.regDeadline)
    : null;
  const deadlinePassed = exam.regDeadline
    ? new Date(exam.regDeadline) < new Date()
    : false;

  const handleClick = () => {
    if (loading) return;
    if (!isRegistered && onRegister) {
      onRegister(exam.id);
    }
  };

  return (
    <CCard className="exam-card-container">
      <CCardBody className="exam-card-body">
        <h5 className="course-name">{exam.courseName}</h5>
        <p className="course-code">{exam.courseCode}</p>

        <div className="exam-details-row">
          <CIcon icon={cilCalendar} size="sm" className="exam-icon" />
          <span>{examDate}</span>
        </div>

        {exam.location && (
          <div className="exam-details-row">
            <CIcon icon={cilLocationPin} size="sm" className="exam-icon" />
            <span>{exam.location}</span>
          </div>
        )}

        {regDeadlineText && (
          <div className="exam-details-row">
            <CIcon icon={cilAlarm} size="sm" className="exam-icon" />
            <span>Deadline: {regDeadlineText}</span>
          </div>
        )}

        <div className="mt-auto">
          {isRegistered ? (
            <div className="mt-3">
              <div className="mb-2">
                <CBadge className="status-badge status-registered">
                  <CIcon icon={cilCheckCircle} size="sm" className="me-1" /> REGISTERED
                </CBadge>
              </div>
              {exam.registrationDate && (
                <div className="text-center pt-2 border-top">
                  <span className="text-muted small">
                    Enrolled on {formatDateOnly(exam.registrationDate)}
                  </span>
                </div>
              )}
            </div>
          ) : (
            <div className="w-100">
              {deadlinePassed && (
                <div className="mb-2">
                  <CBadge className="status-badge status-deadline">
                    DEADLINE PASSED
                  </CBadge>
                </div>
              )}
              <CButton
                className="register-button"
                disabled={loading || deadlinePassed}
                onClick={handleClick}
              >
                {loading ? <CSpinner size="sm" /> : "Register"}
              </CButton>
            </div>
          )}
        </div>
      </CCardBody>
    </CCard>
  );
}
