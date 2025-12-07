// src/features/student/layout/StudentLayout.tsx
import { Outlet, NavLink } from 'react-router-dom';
import {
    CSidebar,
    CSidebarBrand,
    CSidebarNav,
    CNavItem,
    CNavTitle,
    CContainer,
    CAvatar,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import {
    cilSpeedometer,
    cilFile,
    cilChartLine,
    cilClipboard,
    cilEducation,
    cilUser,
    cilLifeRing,
    cilAccountLogout,
} from '@coreui/icons';
import { useAuthContext } from '../../../init/auth';
import './StudentLayout.css';

export function StudentLayout() {
    const { authInfo, logout } = useAuthContext();

    return (
        <div className="student-portal-wrapper">
            <CSidebar className="student-sidebar" position="fixed">
                <CSidebarBrand className="sidebar-brand-wrapper">
                    <div className="unsa-logo">
                        <svg width="40" height="40" viewBox="0 0 40 40" fill="none">
                            <rect width="40" height="40" rx="8" fill="white" />
                            <path d="M20 10L12 15V25L20 30L28 25V15L20 10Z" fill="#0052A5" />
                            <rect x="17" y="18" width="6" height="8" fill="white" />
                        </svg>
                    </div>
                    <div className="brand-text">
                        <span className="brand-name">UNSA</span>
                        <span className="brand-subtitle">SMS</span>
                    </div>
                </CSidebarBrand>

                <CSidebarNav>
                    <CNavTitle>General</CNavTitle>

                    <CNavItem>
                        <NavLink to="/student/dashboard" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilSpeedometer} />
                            Student dashboard
                        </NavLink>
                    </CNavItem>

                    <CNavItem>
                        <NavLink to="/student/document-center" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilFile} />
                            Document center
                        </NavLink>
                    </CNavItem>

                    <CNavItem>
                        <NavLink to="/student/analytics" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilChartLine} />
                            Student analytics
                        </NavLink>
                    </CNavItem>

                    <CNavItem>
                        <NavLink to="/student/request-management" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilClipboard} />
                            Request management
                        </NavLink>
                    </CNavItem>

                    <CNavItem>
                        <NavLink to="/student/enrollment" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilEducation} />
                            Enrollment
                        </NavLink>
                    </CNavItem>

                    <CNavTitle>Settings</CNavTitle>

                    <CNavItem>
                        <NavLink to="/student/profile-settings" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilUser} />
                            Student profile settings
                        </NavLink>
                    </CNavItem>

                    <CNavTitle>Help</CNavTitle>

                    <CNavItem>
                        <NavLink to="/student/support" className="nav-link">
                            <CIcon customClassName="nav-icon" icon={cilLifeRing} />
                            Student support
                        </NavLink>
                    </CNavItem>
                </CSidebarNav>

                <div className="sidebar-footer">
                    <div className="user-profile">
                        <CAvatar color="primary" textColor="white" className="user-avatar">
                            {authInfo?.fullName?.charAt(0) || 'J'}
                        </CAvatar>
                        <div className="user-info">
                            <span className="user-name">{authInfo?.fullName || 'Jane Smith'}</span>
                        </div>
                        <button
                            className="logout-btn"
                            onClick={logout}
                            aria-label="Logout"
                        >
                            <CIcon icon={cilAccountLogout} size="lg" />
                        </button>
                    </div>
                </div>
            </CSidebar>

            <div className="student-portal-content">
                <CContainer fluid className="portal-body">
                    <Outlet />
                </CContainer>
            </div>
        </div>
    );
}