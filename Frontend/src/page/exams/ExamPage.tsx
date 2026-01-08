import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  CAlert,
  CButton,
  CCard,
  CCardBody,
  CCol,
  CContainer,
  CTable,
  CTableBody,
  CTableDataCell,
  CTableHead,
  CTableHeaderCell,
  CTableRow,
  CModal,
  CModalBody,
  CModalFooter,
  CModalHeader,
  CModalTitle,
} from '@coreui/react';

import '../../styles/coreui-custom.css';
import { useAPI } from '../../context/services.tsx';

type Exam = {
  id: number;
  courseName?: string;
  examDate?: string;
  location?: string;
};

export function ExamPage() {
  const navigate = useNavigate();
  const api = useAPI();

  const [exams, setExams] = useState<Exam[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');

  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  const [deleteTarget, setDeleteTarget] = useState<Exam | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  const displayDateTime = (value: string) => {
    if (!value) return '';
    // supports "YYYY-MM-DDTHH:mm" and "YYYY-MM-DD HH:mm"
    return value.replace('T', ' ');
  };

  const loadExams = async () => {
    try {
      setLoading(true);
      setError('');

      const data = await api.getExams();
      const normalized: Exam[] = (data ?? []).map((e) => ({
        id: e.id,
        courseName: e.courseName ?? '',
        examDate: e.examDate ?? '',
        location: e.location ?? '',
      }));
      setExams(normalized);

    } catch (e) {
      console.error(e);
      setError('Failed to load exams.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // toast flag from create
    const toast = sessionStorage.getItem('exams.toast');
    if (toast === 'created') {
      setSuccessMsg('Exam created successfully.');
      sessionStorage.removeItem('exams.toast');
    } else if (toast === 'updated') {
      setSuccessMsg('Exam updated successfully.');
      sessionStorage.removeItem('exams.toast');
    }

    loadExams();
  }, []);

  const openDeleteModal = (exam: Exam) => {
    setDeleteError(null);
    setDeleteTarget(exam);
  };

  const closeDeleteModal = () => {
    setDeleteError(null);
    setDeleteTarget(null);
    setDeleting(false);
  };

  const confirmDelete = async () => {
    if (!deleteTarget) return;

    try {
      setDeleting(true);
      setDeleteError(null);

      await api.deleteExam(deleteTarget.id);
      setExams((prev) => prev.filter((e) => e.id !== deleteTarget.id));
      setSuccessMsg('Exam deleted successfully.');

      closeDeleteModal();
    } catch (e: any) {
      console.error(e);
      setDeleteError(e?.message ?? 'Failed to delete exam.');
      setDeleting(false);
    }
  };

  return (
    <CContainer fluid className="d-flex justify-content-center">
      <CCol xs={12} className="d-flex flex-column align-items-center">
        {/* TOP CARD (like /users) */}
        <CCard className="filter-card">
          <CCardBody>
            <div className="faculty-role-row" style={{ justifyContent: 'space-between' }}>
              <div className="faculty-col">
                <h3 className="mb-0" style={{ fontWeight: 700 }}>
                  Exam Management
                </h3>
              </div>

              <div className="faculty-col d-flex align-items-end">
                <CButton className="btn-blue" onClick={() => navigate('/faculty/exams/create')}>
                  + Create exam
                </CButton>
              </div>
            </div>

            {successMsg && (
              <CAlert color="success" className="mb-0" dismissible onClose={() => setSuccessMsg(null)}>
                {successMsg}
              </CAlert>
            )}

            {error && (
              <CAlert color="danger" className="mb-0">
                {error}
              </CAlert>
            )}
          </CCardBody>
        </CCard>

        {/* TABLE CARD */}
        <CCard className="table-card">
          <CCardBody>
            {loading && <p>Loading...</p>}
            {!loading && !error && exams.length === 0 && <p>No exams found.</p>}

            {!loading && !error && exams.length > 0 && (
              <CTable hover responsive bordered className="user-table">
                <CTableHead color="light">
                  <CTableRow>
                    <CTableHeaderCell>Course</CTableHeaderCell>
                    <CTableHeaderCell>Date &amp; Time</CTableHeaderCell>
                    <CTableHeaderCell>Location</CTableHeaderCell>
                    <CTableHeaderCell className="text-end">Actions</CTableHeaderCell>
                  </CTableRow>
                </CTableHead>

                <CTableBody>
                  {exams.map((exam) => (
                    <CTableRow key={exam.id}>
                      <CTableDataCell>{exam.courseName}</CTableDataCell>
                      <CTableDataCell>{displayDateTime(exam.examDate ?? '')}</CTableDataCell>
                      <CTableDataCell>{exam.location}</CTableDataCell>

                      <CTableDataCell className="text-end">
                        <div className="action-btns" style={{ justifyContent: 'flex-end' }}>
                          <CButton
                            size="sm"
                            color="info"
                            variant="outline"
                            onClick={() => navigate(`/faculty/exams/${exam.id}/edit`)}
                          >
                            Edit
                          </CButton>

                          <CButton
                            size="sm"
                            color="danger"
                            variant="outline"
                            onClick={() => openDeleteModal(exam)}
                          >
                            Delete
                          </CButton>
                        </div>
                      </CTableDataCell>
                    </CTableRow>
                  ))}
                </CTableBody>
              </CTable>
            )}
          </CCardBody>
        </CCard>

        {/* DELETE MODAL
            ✅ ključ: dodaj className="modal-super-high-zindex"
            jer ti je taj stil već definisan globalno u coreui-custom.css
        */}
        <CModal
          visible={!!deleteTarget}
          onClose={closeDeleteModal}
          alignment="center"
          className="modal-super-high-zindex"
        >
          <CModalHeader>
            <CModalTitle>Confirm deletion</CModalTitle>
          </CModalHeader>

          <CModalBody>
            {deleteError && (
              <CAlert color="danger" className="mb-3">
                {deleteError}
              </CAlert>
            )}

            <p>
              Are you sure you want to delete this exam
              {deleteTarget ? ` for "${deleteTarget.courseName}"` : ''}?
            </p>
          </CModalBody>

          <CModalFooter>
            <CButton color="secondary" onClick={closeDeleteModal} disabled={deleting}>
              Cancel
            </CButton>
            <CButton color="primary" onClick={confirmDelete} disabled={deleting}>
              Delete
            </CButton>
          </CModalFooter>
        </CModal>
      </CCol>
    </CContainer>
  );
}