import React, { useEffect, useMemo, useState } from "react";
import {
  CAlert,
  CButton,
  CCard,
  CCardBody,
  CCardHeader,
  CCol,
  CForm,
  CRow,
  CSpinner,
  CTable,
  CTableBody,
  CTableDataCell,
  CTableHead,
  CTableHeaderCell,
  CTableRow,
} from "@coreui/react";
import type { Enrollment } from "../../models/enrollment/Enrollment.types";
import { studentEnrollmentService } from "../../service/enrollment/studentEnrollmentService";

const getCurrentAcademicYear = (date = new Date()): string => {
  const year = date.getFullYear();
  const month = date.getMonth(); // 0=Jan ... 8=Sep

  const startYear = month >= 8 ? year : year - 1;
  return `${startYear}/${startYear + 1}`;
};

export const EnrollmentStudentPage: React.FC = () => {

  const [enrollments, setEnrollments] = useState<Enrollment[]>([]);

  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const CURRENT_ACADEMIC_YEAR = useMemo(() => getCurrentAcademicYear(), []);

  const { activeEnrollments, previousEnrollments } = useMemo(() => {
    const active = enrollments.filter((e) => e.status !== "Done");
    const previous = enrollments.filter((e) => e.status === "Done");
    return { activeEnrollments: active, previousEnrollments: previous };
  }, [enrollments]);

  const loadData = async () => {
    try {
      setIsLoading(true);

      const enrollmentsRes = await studentEnrollmentService.getEnrollments();

      setEnrollments(enrollmentsRes);
    } catch (e) {
      console.error(e);
      setErrorMessage("Failed to load enrollment data.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setIsSubmitting(true);
      setErrorMessage(null);
      setSuccessMessage(null);

      await studentEnrollmentService.createEnrollment(CURRENT_ACADEMIC_YEAR);

      setSuccessMessage(
        `Your enrollment request for ${CURRENT_ACADEMIC_YEAR} has been submitted.`
      );

      await loadData();
    } catch (e) {
      console.error(e);
      setErrorMessage("Failed to submit enrollment request.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="d-flex justify-content-center align-items-center h-100">
        <CSpinner />
      </div>
    );
  }

  return (
    <div className="student-enrollment-page px-4 py-3">
      <h2 style={{ color: "#1e4d8b" }} className="mb-4 fw-semibold">
        Welcome back, Jane!
      </h2>

      <CCard className="mb-4 shadow-sm border-0">
        <CCardHeader className="bg-white border-0 pb-0">
          <h5 style={{ color: "#1e4d8b" }} className="fw-semibold mb-0">
            Enrollment for Academic Year {CURRENT_ACADEMIC_YEAR}
          </h5>
        </CCardHeader>

        <CCardBody>
          {successMessage && (
            <CAlert color="success" className="mb-3">
              {successMessage}
            </CAlert>
          )}
          {errorMessage && (
            <CAlert color="danger" className="mb-3">
              {errorMessage}
            </CAlert>
          )}

          <CForm onSubmit={handleSubmit}>
            <CRow className="align-items-end">
              <CCol className="text-end">
                <CButton
                  type="submit"
                  style={{
                    backgroundColor: "#1e4d8b",
                    borderColor: "#1e4d8b",
                    color: "white",
                  }}
                  className="px-4"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? <CSpinner size="sm" /> : "Submit"}
                </CButton>
              </CCol>
            </CRow>
          </CForm>
        </CCardBody>
      </CCard>

      <CCard className="mb-3 shadow-sm border-0">
        <CCardHeader className="bg-white border-0 pb-0">
          <h5 style={{ color: "#1e4d8b" }} className="fw-semibold mb-2">
            Active enrollments
          </h5>
        </CCardHeader>
        <CCardBody>
          <CTable hover responsive>
            <CTableHead className="bg-light">
              <CTableRow>
                <CTableHeaderCell>Faculty</CTableHeaderCell>
                <CTableHeaderCell>Date</CTableHeaderCell>
                <CTableHeaderCell>Academic Year</CTableHeaderCell>
                <CTableHeaderCell>Status</CTableHeaderCell>
              </CTableRow>
            </CTableHead>
            <CTableBody>
              {activeEnrollments.length === 0 ? (
                <CTableRow>
                  <CTableDataCell colSpan={4} className="text-center text-muted">
                    No active enrollments.
                  </CTableDataCell>
                </CTableRow>
              ) : (
                activeEnrollments.map((e) => (
                  <CTableRow key={e.id}>
                    <CTableDataCell>{e.facultyName}</CTableDataCell>
                    <CTableDataCell>
                      {new Date(e.date).toLocaleDateString()}
                    </CTableDataCell>
                    <CTableDataCell>{e.academicYear}</CTableDataCell>
                    <CTableDataCell>{e.status}</CTableDataCell>
                  </CTableRow>
                ))
              )}
            </CTableBody>
          </CTable>
        </CCardBody>
      </CCard>

      <CCard className="shadow-sm border-0">
        <CCardHeader className="bg-white border-0 pb-0">
          <h5 style={{ color: "#1e4d8b" }} className="fw-semibold mb-2">
            Previous enrollments
          </h5>
        </CCardHeader>
        <CCardBody>
          <CTable hover responsive>
            <CTableHead className="bg-light">
              <CTableRow>
                <CTableHeaderCell>Faculty</CTableHeaderCell>
                <CTableHeaderCell>Date</CTableHeaderCell>
                <CTableHeaderCell>Academic Year</CTableHeaderCell>
                <CTableHeaderCell>Status</CTableHeaderCell>
              </CTableRow>
            </CTableHead>
            <CTableBody>
              {previousEnrollments.length === 0 ? (
                <CTableRow>
                  <CTableDataCell colSpan={4} className="text-center text-muted">
                    No previous enrollments.
                  </CTableDataCell>
                </CTableRow>
              ) : (
                previousEnrollments.map((e) => (
                  <CTableRow key={e.id}>
                    <CTableDataCell>{e.facultyName}</CTableDataCell>
                    <CTableDataCell>
                      {new Date(e.date).toLocaleDateString()}
                    </CTableDataCell>
                    <CTableDataCell>{e.academicYear}</CTableDataCell>
                    <CTableDataCell>{e.status}</CTableDataCell>
                  </CTableRow>
                ))
              )}
            </CTableBody>
          </CTable>
        </CCardBody>
      </CCard>
    </div>
  );
};
