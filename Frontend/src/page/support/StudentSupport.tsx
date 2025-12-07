import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './StudentSupport.css';

export function StudentSupport() {
  return (
    <div className="page-container">
      <h1 className="page-title">Student support</h1>
      <CCard className="content-card">
        <CCardHeader>
          <strong>Help & Support</strong>
        </CCardHeader>
        <CCardBody>
          <p>Support resources and help documentation will be available here.</p>
        </CCardBody>
      </CCard>
    </div>
  );
}