import {
  CCard,
  CCardBody,
  CCardHeader,
  CFormInput,
  CFormSelect,
  CButton,
  CRow,
  CCol,
  CModal,
  CModalHeader,
  CModalBody,
  CModalFooter,
  CFormLabel,
} from '@coreui/react'
import { useState, useEffect } from 'react'
import { useAuthContext } from '../../init/auth'
import './ProfileSettings.css'
import { useAPI } from '../../context/services'

interface UserProfile {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  studentId: string;
  birthDate: string;
  address: string;
  program: string;
}

export function ProfileSettings() {
  const { authInfo } = useAuthContext();
  const api = useAPI();
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [verificationMethod, setVerificationMethod] =
    useState<'email' | 'sms'>('email')

  const [showResetModal, setShowResetModal] = useState(false)
  const [showSuccessModal, setShowSuccessModal] = useState(false)

  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const [userProfile, setUserProfile] = useState<UserProfile>({
    firstName: '',
    lastName: '',
    email: '',
    phone: 'N/A', // Not available in Identity API - at least I don't see it
    studentId: '',
    birthDate: 'N/A', // Not available in Identity API - at least I don't see it
    address: 'N/A', // Not available in Identity API - at least I don't see it
    program: 'N/A', // Not available in Identity API - at least I don't see it
  });

  useEffect(() => {
    if (authInfo) {
      // pre-fill from auth token while loading
      setUserProfile(prev => ({
        ...prev,
        firstName: authInfo.fullName.split(' ')[0] || '',
        lastName: authInfo.fullName.split(' ').slice(1).join(' ') || '',
        email: authInfo.email,
      }));

      //  details from API not hardocded
      api.getCurrentUser()
        .then(data => {
          //  console.log(data);
          setUserProfile(prev => ({
            ...prev,
            firstName: data.firstName || prev.firstName,
            lastName: data.lastName || prev.lastName,
            studentId: data.indexNumber || 'N/A',
            // in case email not in response since using username
            email: data.email || authInfo.email,
          }));
        })
        .catch(err => console.error(err));
    }
  }, [authInfo]);


  const handleSavePassword = async () => {
    setErrorMessage(null)

    if (!password || !confirmPassword) {
      setErrorMessage('All password fields are required.')
      return
    }

    if (password !== confirmPassword) {
      setErrorMessage('Passwords do not match.')
      return
    }

    if (password.length < 8) {
      setErrorMessage('Password must be at least 8 characters long.')
      return
    }


    setShowResetModal(false)

    try {
      await api.changePassword({ newPassword: password });

      setShowSuccessModal(true)
      setPassword('')
      setConfirmPassword('')
      setVerificationMethod('email')
      setErrorMessage(null)
    } catch (err) {
      setErrorMessage(err instanceof Error ? err.message : 'Failed to update password');
      setShowResetModal(true); // Re-open modal if it failed
    }
  }

  const closeResetModal = () => {
    setShowResetModal(false)
    setErrorMessage(null)
    setPassword('')
    setConfirmPassword('')
  }

  return (
    <div className="page-container">
      <h1 className="page-title">Profile settings</h1>

      {/* PROFILE */}
      <CCard className="content-card mb-4">
        <CCardBody className="profile-header">
          <div id="profile-avatar-gigantic">
            {userProfile.firstName && userProfile.firstName[0]}
            {userProfile.lastName && userProfile.lastName[0]}
          </div>
          <div className="profile-info">
            <h2>
              {userProfile.firstName} {userProfile.lastName}
            </h2>
            <p>{userProfile.email}</p>
            {userProfile.studentId && userProfile.studentId !== 'N/A' && <p>Student ID: {userProfile.studentId}</p>}
          </div>
        </CCardBody>
      </CCard>

      {/* USER SETTINGS */}
      <CCard className="content-card mb-4">
        <CCardHeader>
          <strong>User settings</strong>
        </CCardHeader>
        <CCardBody>
          <CRow className="g-3">
            <CCol md={6}>
              <CFormLabel>First name</CFormLabel>
              <CFormInput value={userProfile.firstName} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormLabel>Last name</CFormLabel>
              <CFormInput value={userProfile.lastName} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormLabel>Email</CFormLabel>
              <CFormInput value={userProfile.email} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormLabel>Phone</CFormLabel>
              <CFormInput value={userProfile.phone} readOnly />
            </CCol>
            {userProfile.studentId && userProfile.studentId !== 'N/A' && (
              <CCol md={6}>
                <CFormLabel>Student ID</CFormLabel>
                <CFormInput value={userProfile.studentId} readOnly />
              </CCol>
            )}
            <CCol md={6}>
              <CFormLabel>Birth date</CFormLabel>
              <CFormInput value={userProfile.birthDate} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormLabel>Address</CFormLabel>
              <CFormInput value={userProfile.address} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormLabel>Program</CFormLabel>
              <CFormInput value={userProfile.program} readOnly />
            </CCol>
          </CRow>
        </CCardBody>
      </CCard>

      {/* SECURITY */}
      <CCard className="content-card">
        <CCardHeader>
          <strong>Security settings</strong>
        </CCardHeader>
        <CCardBody>
          <CButton
            color="primary"
            onClick={() => {
              setShowResetModal(true)
              setErrorMessage(null)
            }}
          >
            Reset password
          </CButton>
        </CCardBody>
      </CCard>

      {/* RESET PASSWORD MODAL */}
      <CModal
        visible={showResetModal}
        onClose={closeResetModal}
        alignment="center"
        backdrop={false}
        focus={false}
      >
        <CModalHeader>
          <strong>Reset password</strong>
        </CModalHeader>

        <CModalBody>
          <CFormInput
            type="password"
            label="New password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="mb-3"
          />

          <CFormInput
            type="password"
            label="Confirm password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            className="mb-2"
          />

          {errorMessage && (
            <div className="text-danger mt-1">{errorMessage}</div>
          )}

          <CFormSelect
            label="Verification method"
            value={verificationMethod}
            onChange={(e) =>
              setVerificationMethod(e.target.value as 'email' | 'sms')
            }
            options={[
              { label: 'Email', value: 'email' },
              { label: 'SMS', value: 'sms' },
            ]}
            className="mt-3"
          />
        </CModalBody>

        <CModalFooter>
          <CButton color="secondary" onClick={closeResetModal}>
            Cancel
          </CButton>
          <CButton color="primary" onClick={handleSavePassword}>
            Save
          </CButton>
        </CModalFooter>
      </CModal>

      {/* SUCCESS MODAL */}
      <CModal
        visible={showSuccessModal}
        onClose={() => setShowSuccessModal(false)}
        alignment="center"
        backdrop={false}
        focus={false}
      >
        <CModalHeader>
          <strong>Success</strong>
        </CModalHeader>
        <CModalBody>Password updated successfully.</CModalBody>
        <CModalFooter>
          <CButton color="primary" onClick={() => setShowSuccessModal(false)}>
            OK
          </CButton>
        </CModalFooter>
      </CModal>
    </div>
  )
}
