import {
  CCard,
  CCardBody,
  CCardHeader,
  CForm,
  CFormInput,
  CFormLabel,
  CRow,
  CCol,
  CButton,
  CDropdown,
  CDropdownToggle,
  CDropdownMenu,
  CDropdownItem,
  CTable,
  CTableRow,
  CTableHead,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
  CBadge
} from '@coreui/react';
import "./DocumentCenter.css";

export default function DocumentCenter() {
  return (
    <div className="document-center-container">
      <CCard className="shadow-sm">
        <CCardHeader >
          <h4>Create New Request</h4>
        </CCardHeader>

        <CCardBody>
          <CForm>

            <CRow className="mb-3">
              <CCol md={6}>
                <CFormLabel className="mb-1">Request Type</CFormLabel>

                  <CDropdown className="w-100">
                    <CDropdownToggle color="secondary" className="w-100 text-start">
                      Choose request type
                    </CDropdownToggle>

                    <CDropdownMenu>
                      <CDropdownItem>Action</CDropdownItem>
                      <CDropdownItem>Another action</CDropdownItem>
                      <CDropdownItem>Something else here</CDropdownItem>
                    </CDropdownMenu>
                  </CDropdown>
              </CCol>

              <CCol md={6}>
                <CFormLabel className="mb-1">Status Request</CFormLabel>

                  <CDropdown className="w-100">
                    <CDropdownToggle color="secondary" className="w-100 text-start">
                      Choose status request
                    </CDropdownToggle>

                    <CDropdownMenu>
                      <CDropdownItem>Action</CDropdownItem>
                      <CDropdownItem>Another action</CDropdownItem>
                      <CDropdownItem>Something else here</CDropdownItem>
                    </CDropdownMenu>
                  </CDropdown>
              </CCol>
            </CRow>

            <CRow className="mb-3">
              <CCol md={12}>
                <CFormLabel>Request details</CFormLabel>
                <CFormInput placeholder="Enter request details" />
              </CCol>
            </CRow>

            <div className="text-end">
              <CButton color="secondary">Cancel</CButton>
              <CButton color="primary" className="ms-2">Submit Request</CButton>
            </div>

          </CForm>
        </CCardBody>
      </CCard>

      <CCard className="shadow-sm mt-4">
        <CCardHeader>
          <h4>Active Requests</h4>
        </CCardHeader>

        <CCardBody>
          <CTable striped hover responsive>
            <CTableHead>
              <CTableRow>
                <CTableHeaderCell scope="col">ID</CTableHeaderCell>
                <CTableHeaderCell scope="col">Request Type</CTableHeaderCell>
                <CTableHeaderCell scope="col">Status</CTableHeaderCell>
                <CTableHeaderCell scope="col">Created</CTableHeaderCell>
              </CTableRow>
            </CTableHead>

            <CTableBody>
              <CTableRow>
                <CTableHeaderCell scope="row">1</CTableHeaderCell>
                <CTableDataCell>Enrollment Certificate</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="warning" className="status-badge">Pending</CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-12</CTableDataCell>
              </CTableRow>

              <CTableRow>
                <CTableHeaderCell scope="row">2</CTableHeaderCell>
                <CTableDataCell>Transcript</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="success" className="status-badge">Approved</CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-08</CTableDataCell>
              </CTableRow>
            </CTableBody>
          </CTable>
        </CCardBody>
      </CCard>

      <CCard className="shadow-sm mt-4">
        <CCardHeader>
          <h4>Active Requests</h4>
        </CCardHeader>

        <CCardBody>
          <CTable striped hover responsive>
            <CTableHead>
              <CTableRow>
                <CTableHeaderCell scope="col">ID</CTableHeaderCell>
                <CTableHeaderCell scope="col">Request Type</CTableHeaderCell>
                <CTableHeaderCell scope="col">Status</CTableHeaderCell>
                <CTableHeaderCell scope="col">Created</CTableHeaderCell>
              </CTableRow>
            </CTableHead>

            <CTableBody>
              <CTableRow>
                <CTableHeaderCell scope="row">1</CTableHeaderCell>
                <CTableDataCell>Enrollment Certificate</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="warning" className="status-badge">Pending</CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-12</CTableDataCell>
              </CTableRow>

              <CTableRow>
                <CTableHeaderCell scope="row">2</CTableHeaderCell>
                <CTableDataCell>Transcript</CTableDataCell>
                <CTableDataCell>
                  <CBadge color="success" className="status-badge">Approved</CBadge>
                </CTableDataCell>
                <CTableDataCell>2025-01-08</CTableDataCell>
              </CTableRow>
            </CTableBody>
          </CTable>
        </CCardBody>
      </CCard>
    </div>
  );
}
