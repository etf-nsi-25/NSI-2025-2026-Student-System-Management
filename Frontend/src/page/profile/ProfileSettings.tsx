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
}