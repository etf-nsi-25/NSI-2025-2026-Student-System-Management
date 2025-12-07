import { CCard, CCardBody, CCardHeader } from '@coreui/react';
import './RequestManagement.css';

export function RequestManagement() {
    return (
        <div className="page-container">
            <h1 className="page-title">Request management</h1>
            <CCard className="content-card">
                <CCardHeader>
                    <strong>My Requests</strong>
                </CCardHeader>
                <CCardBody>
                    <p>Request tracking and management interface will be available here.</p>
                </CCardBody>
            </CCard>
        </div>

    );
}