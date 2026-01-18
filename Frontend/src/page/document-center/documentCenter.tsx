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
  CAlert,
  CSpinner,
} from '@coreui/react';
import './documentCenter.css';
import { useAPI } from '../../context/services.tsx';

type ValidationErrors = {
  userId?: string;
  facultyId?: string;
  requestType?: string;
  status?: string;
  details?: string;
};

export default function DocumentCenterDashboard() {
  const [requestType, setRequestType] = useState('');
  const [status, setStatus] = useState('');
  const [details, setDetails] = useState('');

  const [userId, setUserId] = useState('');
  const [facultyId, setFacultyId] = useState('');

  const [errors, setErrors] = useState<ValidationErrors>({});
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState('');
  const [submitSuccess, setSubmitSuccess] = useState('');

  const api = useAPI();

  const validate = (): ValidationErrors => {
    const newErrors: ValidationErrors = {};

    if (!userId.trim()) newErrors.userId = 'UserId is required.';
    if (!facultyId.trim()) newErrors.facultyId = 'FacultyId is required.';
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
      userId,
      facultyId,
      documentType: requestType,
      status,
    };

    try {
      setSubmitting(true);
      await api.post('/api/Support/document-request', payload);
      setSubmitSuccess('Request successfully submitted.');
      setRequestType('');
      setStatus('');
      setDetails('');
      setFacultyId('');
      setUserId('');
    } catch (err) {
      const error = err as Error;
      setSubmitError(error.message || 'Error while submitting request.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleCancel = () => {
    setRequestType('');
    setStatus('');
    setDetails('');
    setFacultyId('');
    setUserId('');
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
          {submitError && <CAlert color="danger">{submitError}</CAlert>}
          {submitSuccess && <CAlert color="success">{submitSuccess}</CAlert>}

          <CForm onSubmit={handleSubmit}>
            <CRow className="mb-3">
              <CCol md={6}>
                <CFormLabel>User Id</CFormLabel>
                <CFormInput
                  value={userId}
                  onChange={(e) => setUserId(e.target.value)}
                  placeholder="Enter user id"
                />
                {errors.userId && <div className="text-danger small">{errors.userId}</div>}
              </CCol>

              <CCol md={6}>
                <CFormLabel>Faculty Id (GUID)</CFormLabel>
                <CFormInput
                  type="text"
                  value={facultyId}
                  onChange={(e) => setFacultyId(e.target.value)}
                  placeholder="Enter faculty GUID"
                />
                {errors.facultyId && <div className="text-danger small">{errors.facultyId}</div>}
              </CCol>
            </CRow>

            <CRow className="mb-3">
              <CCol md={6}>
                <CFormLabel>Request Type</CFormLabel>
                <CDropdown className="w-100">
                  <CDropdownToggle color="secondary" className="w-100 text-start">
                    {requestType || 'Choose request type'}
                  </CDropdownToggle>
                  <CDropdownMenu>
                    <CDropdownItem onClick={() => setRequestType('Enrollment Certificate')}>
                      Enrollment Certificate
                    </CDropdownItem>
                    <CDropdownItem onClick={() => setRequestType('Transcript')}>
                      Transcript
                    </CDropdownItem>
                    <CDropdownItem onClick={() => setRequestType('Other document')}>
                      Other document
                    </CDropdownItem>
                  </CDropdownMenu>
                </CDropdown>
                {errors.requestType && <div className="text-danger small">{errors.requestType}</div>}
              </CCol>

              <CCol md={6}>
                <CFormLabel>Status</CFormLabel>
                <CDropdown className="w-100">
                  <CDropdownToggle color="secondary" className="w-100 text-start">
                    {status || 'Choose status'}
                  </CDropdownToggle>
                  <CDropdownMenu>
                    <CDropdownItem onClick={() => setStatus('Pending')}>Pending</CDropdownItem>
                    <CDropdownItem onClick={() => setStatus('Approved')}>Approved</CDropdownItem>
                    <CDropdownItem onClick={() => setStatus('Rejected')}>Rejected</CDropdownItem>
                  </CDropdownMenu>
                </CDropdown>
                {errors.status && <div className="text-danger small">{errors.status}</div>}
              </CCol>
            </CRow>

            <CRow className="mb-3">
              <CCol md={12}>
                <CFormLabel>Request details</CFormLabel>
                <CFormInput
                  value={details}
                  onChange={(e) => setDetails(e.target.value)}
                  placeholder="Enter request details"
                />
                {errors.details && <div className="text-danger small">{errors.details}</div>}
              </CCol>
            </CRow>

            <div className="text-end">
              <CButton color="secondary" onClick={handleCancel} disabled={submitting}>
                Cancel
              </CButton>
              <CButton type="submit" color="primary" className="ms-2" disabled={submitting}>
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
    </div>
  );
}