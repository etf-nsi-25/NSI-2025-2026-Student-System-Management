import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './Enrollment.css';

export function Enrollment() {
  return (
    <div className="page-container">
      <h1 className="page-title">Enrollment</h1>
      <CCard className="content-card">
        <CCardHeader>
          <strong>Course Enrollment</strong>
        </CCardHeader>
        <CCardBody>
          <p>Course enrollment and registration interface will be displayed here.</p>
        </CCardBody>
      </CCard>
    </div>
  );
}
