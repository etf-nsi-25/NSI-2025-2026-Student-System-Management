/*Prije bilo
import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './ProfileSettings.css';

export function ProfileSettings() {
  return (
    <div className="page-container">
      <h1 className="page-title">Student profile settings</h1>
      <CCard className="content-card">
        <CCardHeader>
          <strong>Profile Configuration</strong>
        </CCardHeader>
        <CCardBody>
          <p>Profile settings and preferences will be available here.</p>
        </CCardBody>
      </CCard>
    </div>
  );
}*/

/*Moj kod*/
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
} from '@coreui/react'
import { useState } from 'react'
import './ProfileSettings.css'

export function ProfileSettings() {
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [verificationMethod, setVerificationMethod] =
    useState<'email' | 'sms'>('email')

  const [showResetModal, setShowResetModal] = useState(false)
  const [showSuccessModal, setShowSuccessModal] = useState(false)

  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const student = {
    firstName: 'Student',
    lastName: 'User',
    email: 'student.user@example.com',
    phone: '+000 000 000',
    studentId: 'STU000001',
    birthDate: '2000-01-01',
    address: 'N/A',
    program: 'Study Program',
  }

  const handleSavePassword = () => {
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
    setShowSuccessModal(true)

    setPassword('')
    setConfirmPassword('')
    setVerificationMethod('email')
    setErrorMessage(null)
  }

  const closeResetModal = () => {
    setShowResetModal(false)
    setErrorMessage(null)
    setPassword('')
    setConfirmPassword('')
  }

  return (
    <div className="page-container">
      <h1 className="page-title">Student profile settings</h1>

      {/* PROFILE */}
      <CCard className="content-card mb-4">
        <CCardBody className="profile-header">
          <div className="avatar">
            {student.firstName[0]}
            {student.lastName[0]}
          </div>
          <div className="profile-info">
            <h2>
              {student.firstName} {student.lastName}
            </h2>
            <p>{student.email}</p>
            <p>Student ID: {student.studentId}</p>
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
              <CFormInput label="First name" value={student.firstName} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Last name" value={student.lastName} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Email" value={student.email} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Phone" value={student.phone} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Student ID" value={student.studentId} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Birth date" value={student.birthDate} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Address" value={student.address} readOnly />
            </CCol>
            <CCol md={6}>
              <CFormInput label="Program" value={student.program} readOnly />
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
