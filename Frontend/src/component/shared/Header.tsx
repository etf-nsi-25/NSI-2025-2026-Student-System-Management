import { CHeader, CContainer } from '@coreui/react';

export default function Header() {
  return (
    <CHeader className="bg-primary text-white py-3">
      <CContainer className="d-flex justify-content-between align-items-center px-3">
        <img src="/unsa.png" alt="UNSA Logo" style={{ height: 48, width: 112 }} />
        <span className="small">Faculty admin</span>
      </CContainer>
    </CHeader>
  );
}
