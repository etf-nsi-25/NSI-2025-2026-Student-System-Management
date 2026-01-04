// src/component/exams/ExamCard.tsx
import { CCard, CCardBody, CBadge, CButton, CSpinner } from "@coreui/react";
import CIcon from "@coreui/icons-react";
import {
  cilLibrary,
  cilLocationPin,
  cilCalendar,
  cilAlarm,
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

  const buttonLabel = loading ? "Registering..." : "Register";

  return (
    <CCard
      className="ui-surface-glass-card"
      style={{
        opacity: loading ? 0.75 : 1,
        pointerEvents: loading ? "none" : "auto",
        transition: "opacity 0.2s ease",
        borderRadius: 28,
        padding: 28,
        minHeight: 260,
        display: "flex",
        flexDirection: "column",
        justifyContent: "space-between",
        background:
          "linear-gradient(135deg, rgba(33,37,63,0.92), rgba(7,9,18,0.95))",
        backdropFilter: "blur(22px)",
        border: "1px solid rgba(255,255,255,0.16)",
        color: "#E7ECF5",
      }}
    >
      <CCardBody style={{ padding: 0, display: "flex", flexDirection: "column", gap: 18 }}>

        {/* HEADER ROW: badges + status */}
        <div className="d-flex justify-content-between align-items-center mb-2">
          <div className="d-flex gap-2">
            <CBadge className="ui-badge ui-badge-primary d-flex align-items-center gap-1">
              <CIcon icon={cilLibrary} size="sm" /> {exam.courseCode}
            </CBadge>

            {exam.location ? (
              <CBadge className="ui-badge ui-badge-soft d-flex align-items-center gap-1">
                <CIcon icon={cilLocationPin} size="sm" /> {exam.location}
              </CBadge>
            ) : null}
          </div>

          {isRegistered && (
            <span className="ui-badge ui-badge-status">Registered</span>
          )}
        </div>

        {/* TITLE */}
        <h3
          style={{
            fontSize: 22,
            fontWeight: 700,
            margin: 0,
            color: "#FFFFFF",
            minHeight: 52,
          }}
        >
          {exam.courseName}
        </h3>

        {/* INFO BLOCK */}
        <div
          className="d-flex flex-column gap-2"
          style={{ fontSize: 15, color: "#b6c0d1" }}
        >
          <div className="ui-info-pill">
            <CIcon icon={cilCalendar} size="sm" className="ui-info-icon" />
            <span>{examDate}</span>
          </div>

          {regDeadlineText ? (
            <div className="ui-info-pill">
              <CIcon icon={cilAlarm} size="sm" className="ui-info-icon" />
              <span>Deadline: {regDeadlineText}</span>
            </div>
          ) : null}
        </div>

        {/* ACTION ROW */}
        <div
          className="d-flex justify-content-between align-items-center"
          style={{ marginTop: 16 }}
        >
          {!isRegistered ? (
            <>
              <CButton
                className="ui-button-cta"
                style={{
                  minWidth: 120,
                  padding: "10px 18px",
                  fontSize: 13,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  gap: 8,
                }}
                disabled={loading || deadlinePassed}
                onClick={handleClick}
              >
                {loading ? (
                  <>
                    <CSpinner size="sm" />
                    <span>Registering</span>
                  </>
                ) : (
                  buttonLabel.toUpperCase()
                )}
              </CButton>

              {deadlinePassed && (
                <span className="ui-validation-text">Registration deadline has passed.</span>
              )}
            </>
          ) : null}


          {exam.registrationDate && (
            <span className="ui-text-caption">
              Registered on {formatDateOnly(exam.registrationDate)}
            </span>
          )}

        </div>
      </CCardBody>
    </CCard>
  );
}
