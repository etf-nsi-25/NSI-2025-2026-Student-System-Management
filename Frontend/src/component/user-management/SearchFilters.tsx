import { CCard, CCardBody, CFormSelect, CFormInput, CButton, CRow, CCol } from '@coreui/react';
import { ROLE_MAP } from '../../page/userManagement';
interface SearchFiltersProps {
  faculty: string;
  setFaculty: (value: string) => void;
  role: string;
  setRole: (value: string) => void;
  searchTerm: string;
  setSearchTerm: (value: string) => void;
  onAddUser: () => void;
}

export default function SearchFilters({
  faculty,
  setFaculty,
  role,
  setRole,
  searchTerm,
  setSearchTerm,
  onAddUser,
}: SearchFiltersProps) {
  return (
    <CCard>
      <CCardBody>
        <CRow className="mb-3">
          <CCol md={6}>
            <label className="form-label">Faculty</label>
            <CFormSelect value={faculty} onChange={(e: any) => setFaculty(e.target.value)}>
              <option>ETF UNSA</option>
              <option>All</option>
            </CFormSelect>
          </CCol>
          <CCol md={6}>
            <label className="form-label">Role</label>
            <CFormSelect value={role} onChange={(e: any) => setRole(e.target.value)}>
              <option value="All">All</option>
              {Object.entries(ROLE_MAP).map(([k, v]) => (
                <option key={k} value={k}>{v}</option>
              ))}
            </CFormSelect>
          </CCol>
        </CRow>

        <CFormInput
          placeholder="Search for user.."
          value={searchTerm}
          onChange={(e: any) => setSearchTerm(e.target.value)}
          className="mb-3"
        />

        <div className="d-flex justify-content-center gap-2">
          <CButton color="primary">Search</CButton>
          <CButton color="primary" onClick={onAddUser}>+ Add User</CButton>
          <CButton color="primary">+ Add in bulk</CButton>
        </div>
      </CCardBody>
    </CCard>
  );
}
