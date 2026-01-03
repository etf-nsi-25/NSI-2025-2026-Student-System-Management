import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './StudentSupport.css';

export function AssistentSupport() {
  return (
    <div className="page-container">
      <h1 className="page-title">Assistant Support</h1>
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