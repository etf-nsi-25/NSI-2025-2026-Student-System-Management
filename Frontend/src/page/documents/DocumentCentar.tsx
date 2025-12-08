import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './DocumentCentar.css';

export function DocumentCenter() {
  return (
    <div className="page-container">
      <h1 className="page-title">Document center</h1>
      <CCard className="content-card">
        <CCardHeader>
          <strong>My Documents</strong>
        </CCardHeader>
        <CCardBody>
          <p>Document management and downloads will be available here.</p>
        </CCardBody>
      </CCard>
    </div>
  );
}