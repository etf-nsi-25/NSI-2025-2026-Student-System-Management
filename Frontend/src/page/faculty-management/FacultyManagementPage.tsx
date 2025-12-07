import {
  CContainer,
  CHeader,
  CHeaderBrand,
  CSidebar,
  CSidebarNav,
  CNavItem,
} from '@coreui/react';

import logo from '../../assets/images/logo.jpg';
import { FacultyListingPage } from './FacultyListingPage';
import './FacultyManagementPage.css';

// 👇 Backend ti sluša na http://localhost:5000
const FACULTIES_API_BASE_URL = 'http://localhost:5000/api/university/faculties';

export function FacultyManagementPage() {
  return (
    <CContainer fluid className="fm-page">
      {/* HEADER */}
      <CHeader position="sticky" className="fm-header">
        <CHeaderBrand className="fm-header-brand">
          <img src={logo} alt="Logo" className="fm-logo" />
          <span className="fm-header-title">UNSA SMS</span>
        </CHeaderBrand>

        <div className="fm-header-user">Superadmin</div>
      </CHeader>

      {/* BODY */}
      <div className="fm-body">
        {/* SIDEBAR */}
        <CSidebar className="fm-sidebar" unfoldable>
          <CSidebarNav className="fm-sidebar-nav">
            <div className="fm-sidebar-section">Navigation</div>

            <CNavItem className="fm-sidebar-item">
              <span>Dashboard</span>
            </CNavItem>

            <CNavItem className="fm-sidebar-item">
              <span>Course Management</span>
            </CNavItem>

            <CNavItem className="fm-sidebar-item">
              <span>User Management</span>
            </CNavItem>

            <CNavItem className="fm-sidebar-item">
              <span>Tenant Management</span>
            </CNavItem>

            <CNavItem className="fm-sidebar-item">
              <span>Student Support</span>
            </CNavItem>

            <div className="fm-sidebar-section">Settings</div>

            <CNavItem className="fm-sidebar-item">
              <span>Settings</span>
            </CNavItem>

            <CNavItem className="fm-sidebar-item">
              <span>Help</span>
            </CNavItem>
          </CSidebarNav>
        </CSidebar>

        {/* MAIN CONTENT */}
        <main className="fm-content">
          {/* 👇 sad ListingPage dobija URL backend-a */}
          <FacultyListingPage apiBaseUrl={FACULTIES_API_BASE_URL} />
        </main>
      </div>
    </CContainer>
  );
}
