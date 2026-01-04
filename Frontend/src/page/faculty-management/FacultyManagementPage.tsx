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

type Role = 'Superadmin' | 'User';
const CURRENT_USER_ROLE: Role = 'Superadmin'; 

export function FacultyManagementPage() {
  if (CURRENT_USER_ROLE !== 'Superadmin') {
    return (
      <CContainer fluid className="fm-page fm-unauthorized">
        <CHeader position="sticky" className="fm-header">
          <CHeaderBrand className="fm-header-brand">
            <img src={logo} alt="Logo" className="fm-logo" />
            <span className="fm-header-title">UNSA SMS</span>
          </CHeaderBrand>

          <div className="fm-header-user">Guest</div>
        </CHeader>

        <main className="fm-content">
          <div className="fm-unauthorized-box">
            <h2>Unauthorized</h2>
            <p>You do not have permission to access Faculty Management.</p>
          </div>
        </main>
      </CContainer>
    );
  }


  return (
    <CContainer fluid className="fm-page">
      <CHeader position="sticky" className="fm-header">
        <CHeaderBrand className="fm-header-brand">
          <img src={logo} alt="Logo" className="fm-logo" />
          <span className="fm-header-title">UNSA SMS</span>
        </CHeaderBrand>

        <div className="fm-header-user">Superadmin</div>
      </CHeader>

      <div className="fm-body">
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

        <main className="fm-content">
          <FacultyListingPage />
        </main>
      </div>
    </CContainer>
  );
}
