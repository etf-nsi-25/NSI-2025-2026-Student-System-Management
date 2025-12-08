import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './StudentAnalytics.css';

export function StudentAnalytics() {
  return (
    <div className="page-container">
      <h1 className="page-title">Student analytics</h1>
      <CCard className="content-card">
        <CCardHeader>
          <strong>Performance Analytics</strong>
        </CCardHeader>
        <CCardBody>
          <p>Detailed analytics and performance metrics will be displayed here.</p>
        </CCardBody>
      </CCard>
    </div>
  );
}