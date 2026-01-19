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

import { useAPI } from "../../context/services";
import { useAuthContext } from "../../init/auth";
import { studentEnrollmentService } from "../../service/enrollment/studentEnrollmentService";

import type { EnrollmentRequest } from "../../models/enrollment/Enrollment.types";

const getCurrentAcademicYear = (date = new Date()): string => {
  const year = date.getFullYear();
  const month = date.getMonth(); // 0=Jan ... 8=Sep
  const startYear = month >= 8 ? year : year - 1;
  return `${startYear}/${startYear + 1}`;
};

export const EnrollmentStudentPage: React.FC = () => {
  const api = useAPI();
  const { authInfo } = useAuthContext();

  const userId = authInfo?.userId;
  const facultyId = authInfo?.tenantId;

  const [enrollments, setEnrollments] = useState<EnrollmentRequest[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const CURRENT_ACADEMIC_YEAR = useMemo(() => getCurrentAcademicYear(), []);

  const { activeEnrollments, previousEnrollments } = useMemo(() => {
    const active = enrollments.filter((e) => e.status === "Pending");
    const previous = enrollments.filter((e) => e.status !== "Pending"); // Approved/Rejected
    return { activeEnrollments: active, previousEnrollments: previous };
  }, [enrollments]);

  const loadData = async () => {
    try {
      setIsLoading(true);
      setErrorMessage(null);

      if (!userId) {
        setEnrollments([]);
        setErrorMessage("Missing user id. Please login again.");
        return;
      }

      const res = await studentEnrollmentService.getEnrollments(api, userId);
      setEnrollments(res);
    } catch (e) {
      console.error(e);
      setEnrollments([]);
      setErrorMessage("Failed to load enrollment data.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (userId) loadData();
    else setIsLoading(false);
  }, [userId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setIsSubmitting(true);
      setErrorMessage(null);
      setSuccessMessage(null);

      if (!userId || !facultyId) {
        throw new Error("Missing userId/facultyId from auth token.");
      }

      await studentEnrollmentService.createEnrollment(api, {
        userId,
        facultyId,
        academicYear: CURRENT_ACADEMIC_YEAR,
        semester: 1,
      });

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
        Welcome back, {authInfo?.fullName ?? "Student"}!
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
                  disabled={isSubmitting || !userId || !facultyId}
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
            Active enrollment requests
          </h5>
        </CCardHeader>
        <CCardBody>
          <CTable hover responsive>
            <CTableHead className="bg-light">
              <CTableRow>
                <CTableHeaderCell>Date</CTableHeaderCell>
                <CTableHeaderCell>Academic Year</CTableHeaderCell>
                <CTableHeaderCell>Semester</CTableHeaderCell>
                <CTableHeaderCell>Status</CTableHeaderCell>
              </CTableRow>
            </CTableHead>
            <CTableBody>
              {activeEnrollments.length === 0 ? (
                <CTableRow>
                  <CTableDataCell colSpan={4} className="text-center text-muted">
                    No active enrollment requests.
                  </CTableDataCell>
                </CTableRow>
              ) : (
                activeEnrollments.map((e) => (
                  <CTableRow key={e.id}>
                    <CTableDataCell>
                      {new Date(e.createdAt).toLocaleDateString()}
                    </CTableDataCell>
                    <CTableDataCell>{e.academicYear}</CTableDataCell>
                    <CTableDataCell>{e.semester}</CTableDataCell>
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
            Previous enrollment requests
          </h5>
        </CCardHeader>
        <CCardBody>
          <CTable hover responsive>
            <CTableHead className="bg-light">
              <CTableRow>
                <CTableHeaderCell>Date</CTableHeaderCell>
                <CTableHeaderCell>Academic Year</CTableHeaderCell>
                <CTableHeaderCell>Semester</CTableHeaderCell>
                <CTableHeaderCell>Status</CTableHeaderCell>
              </CTableRow>
            </CTableHead>
            <CTableBody>
              {previousEnrollments.length === 0 ? (
                <CTableRow>
                  <CTableDataCell colSpan={4} className="text-center text-muted">
                    No previous enrollment requests.
                  </CTableDataCell>
                </CTableRow>
              ) : (
                previousEnrollments.map((e) => (
                  <CTableRow key={e.id}>
                    <CTableDataCell>
                      {new Date(e.createdAt).toLocaleDateString()}
                    </CTableDataCell>
                    <CTableDataCell>{e.academicYear}</CTableDataCell>
                    <CTableDataCell>{e.semester}</CTableDataCell>
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
