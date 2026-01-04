type ValidationErrors = {
  userId?: string;
  facultyId?: string;
  requestType?: string;
  status?: string;
  details?: string;
};

import { useState } from 'react';
import {
  CCard,
  CCardBody,
  CCardHeader,
  CForm,
  CFormInput,
  CFormLabel,
  CRow,
  CCol,
  CButton,
  CDropdown,
  CDropdownToggle,
  CDropdownMenu,
  CDropdownItem,
  CTable,
  CTableRow,
  CTableHead,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
  CBadge,
  CAlert,
  CSpinner,
} from '@coreui/react';
import './documentCenter.css';
import { useAPI } from '../../context/services.tsx';

export default function DocumentCenterDashboard() {
  const [requestType, setRequestType] = useState('');
  const [status, setStatus] = useState('');
  const [details, setDetails] = useState('');

  const [userId, setUserId] = useState(''); 
  const [facultyId, setFacultyId] = useState(1);

  const [errors, setErrors] = useState<ValidationErrors>({});
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');
  const [submitSuccess, setSubmitSuccess] = useState('');

  const api = useAPI();

  const validate = (): ValidationErrors => {
    const newErrors: ValidationErrors = {};

    if (!userId.trim()) newErrors.userId = 'UserId is required.';
    if (!facultyId || Number(facultyId) <= 0)
      newErrors.facultyId = 'FacultyId must be greater than 0.';
    if (!requestType) newErrors.requestType = 'Request type is required.';
    if (!status) newErrors.status = 'Status is required.';
    if (!details.trim()) newErrors.details = 'Details are required.';

    return newErrors;
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setSubmitError('');
    setSubmitSuccess('');

    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    setErrors({});

    const payload = {
      userId: userId,
      facultyId: Number(facultyId),
      documentType: requestType,
      status: status
    };

    try {
      setSubmitting(true);

      api.post('/api/Support/document-request', payload)
          .then(response => {
            console.log('Created request: ', response);
            setSubmitSuccess('Request successfully submitted.');
            setRequestType('');
            setStatus('');
            setDetails('');
          });
    } catch (err) {
      const error = err as Error;
      setSubmitError(error.message || 'Error while submitting request.');
    }
     finally {
      setSubmitting(false);
    }
  };

  const handleCancel = () => {
    setRequestType('');
    setStatus('');
    setDetails('');
    setErrors({});
    setSubmitError('');
    setSubmitSuccess('');
  };

  return (
    <div className="document-center-container">
      <CCard className="shadow-sm">
        <CCardHeader>
          <h4>Create New Request</h4>
        </CCardHeader>

        <CCardBody>
          {submitError && (
            <CAlert color="danger" className="mb-3">
              {submitError}
            </CAlert>
          )}
          {submitSuccess && (
            <CAlert color="success" className="mb-3">
              {submitSuccess}
            </CAlert>
          )}

          <CForm onSubmit={handleSubmit}>
            <CRow className="mb-3">
              <CCol md={6}>
                <CFormLabel>User Id</CFormLabel>
                <CFormInput
                  value={userId}
                  onChange={(e) => setUserId(e.target.value)}
                  placeholder="Enter user Id"
                />
                {errors.userId && (
                  <div className="text-danger small mt-1">
                    {errors.userId}
                  </div>
                )}
              </CCol>

              <CCol md={6}>
                <CFormLabel>Faculty Id</CFormLabel>
                <CFormInput
                  type="number"
                  value={facultyId}
                  onChange={(e) => setFacultyId(Number(e.target.value))}
                  placeholder="Enter faculty id"
                />
                {errors.facultyId && (
                  <div className="text-danger small mt-1">
                    {errors.facultyId}
                  </div>
                )}
              </CCol>
            </CRow>

            <CRow className="mb-3">
              <CCol md={6}>
                <CFormLabel className="mb-1">Request Type</CFormLabel>

                <CDropdown className="w-100">
                  <CDropdownToggle
                    color="secondary"
                    className="w-100 text-start"
                  >
                    {requestType || 'Choose request type'}
                  </CDropdownToggle>

                  <CDropdownMenu>
                    <CDropdownItem
                      onClick={() => setRequestType('Enrollment Certificate')}
                    >
                      Enrollment Certificate
                    </CDropdownItem>
                    <CDropdownItem
                      onClick={() => setRequestType('Transcript')}
                    >
                      Transcript
                    </CDropdownItem>
                    <CDropdownItem
                      onClick={() => setRequestType('Other document')}
                    >
                      Other document
                    </CDropdownItem>
                  </CDropdownMenu>
                </CDropdown>

                {errors.requestType && (
                  <div className="text-danger small mt-1">
                    {errors.requestType}
                  </div>
                )}
              </CCol>

              <CCol md={6}>
                <CFormLabel className="mb-1">Status Request</CFormLabel>

                <CDropdown className="w-100">
                  <CDropdownToggle
                    color="secondary"
                    className="w-100 text-start"
                  >
                    {status || 'Choose status request'}
                  </CDropdownToggle>

                  <CDropdownMenu>
                    <CDropdownItem onClick={() => setStatus('Pending')}>
                      Pending
                    </CDropdownItem>
                    <CDropdownItem onClick={() => setStatus('Approved')}>
                      Approved
                    </CDropdownItem>
                    <CDropdownItem onClick={() => setStatus('Rejected')}>
                      Rejected
                    </CDropdownItem>
                  </CDropdownMenu>
                </CDropdown>

                {errors.status && (
                  <div className="text-danger small mt-1">
                    {errors.status}
                  </div>
                )}
              </CCol>
            </CRow>

            <CRow className="mb-3">
              <CCol md={12}>
                <CFormLabel>Request details</CFormLabel>
                <CFormInput
                  placeholder="Enter request details"
                  value={details}
                  onChange={(e) => setDetails(e.target.value)}
                />
                {errors.details && (
                  <div className="text-danger small mt-1">
                    {errors.details}
                  </div>
                )}
              </CCol>
            </CRow>

            <div className="text-end">
              <CButton
                type="button"
                color="secondary"
                onClick={handleCancel}
                disabled={submitting}
              >
                Cancel
              </CButton>
              <CButton
                type="submit"
                color="primary"
                className="ms-2"
                disabled={submitting}
              >
                {submitting ? (
                  <>
                    <CSpinner size="sm" className="me-2" />
                    Submitting...
                  </>
                ) : (
                  'Submit Request'
                )}
              </CButton>
            </div>
          </CForm>
        </CCardBody>
      </CCard>

      {}
      <CCard className="shadow-sm mt-4">
        <CCardHeader>
          <h4>Active Requests</h4>
        </CCardHeader>

        <CCardBody>
          <CTable striped hover responsive>
            <CTableHead>
              <CTableRow>
                <CTableHeaderCell scope="col">ID</CTableHeaderCell>
                <CTableHeaderCell scope="col">Request Type</CTableHeaderCell>
                <CTableHeaderCell scope="col">Status</CTableHeaderCell>
                <CTableHeaderCell scope="col">Created</CTableHeaderCell>
              </CTableRow>
            </CTableHead>

            <CTableBody>
              <CTableRow>
                <CTableHeaderCell scope="row">1</CTableHeaderCell>
                <CTableDataCell>Enrollment Certificate</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="warning" className="status-badge">
                    Pending
                  </CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-12</CTableDataCell>
              </CTableRow>

              <CTableRow>
                <CTableHeaderCell scope="row">2</CTableHeaderCell>
                <CTableDataCell>Transcript</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="success" className="status-badge">
                    Approved
                  </CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-08</CTableDataCell>
              </CTableRow>
            </CTableBody>
          </CTable>
        </CCardBody>
      </CCard>

      <CCard className="shadow-sm mt-4">
        <CCardHeader>
          <h4>Previous Requests</h4>
        </CCardHeader>

        <CCardBody>
          <CTable striped hover responsive>
            <CTableHead>
              <CTableRow>
                <CTableHeaderCell scope="col">ID</CTableHeaderCell>
                <CTableHeaderCell scope="col">Request Type</CTableHeaderCell>
                <CTableHeaderCell scope="col">Status</CTableHeaderCell>
                <CTableHeaderCell scope="col">Created</CTableHeaderCell>
              </CTableRow>
            </CTableHead>

            <CTableBody>
              <CTableRow>
                <CTableHeaderCell scope="row">1</CTableHeaderCell>
                <CTableDataCell>Enrollment Certificate</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="warning" className="status-badge">
                    Pending
                  </CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-12</CTableDataCell>
              </CTableRow>

              <CTableRow>
                <CTableHeaderCell scope="row">2</CTableHeaderCell>
                <CTableDataCell>Transcript</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="success" className="status-badge">
                    Approved
                  </CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-08</CTableDataCell>
              </CTableRow>
            </CTableBody>
          </CTable>
        </CCardBody>
      </CCard>

    </div>
  );
}
