import type { User } from "../../types/user-types";
import { CCard, CCardBody, CButton } from '@coreui/react';

interface UserDetailsPanelProps {
  user: User | null;
}

export default function UserDetailsPanel({ user }: UserDetailsPanelProps) {
  if (!user) {
    return (
      <CCard style={{ width: 320, maxHeight: 400 }}>
        <CCardBody className="d-flex align-items-center justify-content-center">
          <p className="text-muted small">Select a user to view details</p>
        </CCardBody>
      </CCard>
    );
  }

  return (
    <CCard style={{ width: 320, maxHeight: 400 }}>
      <CCardBody>
        <h2 className="h5 text-primary">User Details</h2>

        <div className="mb-3">
          <label className="form-label small text-muted">Name:</label>
          <div className="fw-semibold">{user.firstName + " " + user.lastName}</div>
        </div>

        <div className="mb-3">
          <label className="form-label small text-muted">Role:</label>
          <div>{user.role}</div>
        </div>

        <div className="mb-3">
          <label className="form-label small text-muted">Permissions:</label>
          <div className="text-muted small">Edit Courses, Grade Students</div>
        </div>

        <div className="mb-3">
          <label className="form-label small text-muted">Last login:</label>
          <div>{user.lastActive}</div>
        </div>

        <div className="text-end">
          <CButton color="primary">Edit Permissions</CButton>
        </div>
      </CCardBody>
    </CCard>
  );
}
