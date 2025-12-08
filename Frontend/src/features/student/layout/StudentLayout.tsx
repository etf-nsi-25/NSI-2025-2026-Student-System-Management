// src/features/student/layout/StudentLayout.tsx
import { Outlet, NavLink, useNavigate } from 'react-router-dom';
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
    cilMenu,
} from '@coreui/icons';
import { useAuthContext } from '../../../init/auth';
import logo from '../../../assets/images/login/unsa-sms-logo.png';
import './StudentLayout.css';
import { useCallback, useState } from 'react';
import { logoutFromServer, resetAuthInfo } from '../../../utils/authUtils';


export function StudentLayout() {
    const { authInfo, setAuthInfo } = useAuthContext();
    const [sidebarOpen, setSidebarOpen] = useState(true);
    const navigate = useNavigate();


    const logout = useCallback(async () => {

        try {
            await logoutFromServer();
        } catch (err) {
            console.warn("Server logout failed, clearing client session anyway");
        }

        resetAuthInfo(setAuthInfo);

        navigate('/login');
    }, [navigate, setAuthInfo]);

    return (
        <div className="student-portal-wrapper">
            <CSidebar
                className={`student-sidebar ${sidebarOpen ? 'show' : ''}`}
                position="fixed"
            >

                <CSidebarBrand className="sidebar-brand-wrapper">
                    <div className="unsa-logo">
                        <img src={logo} alt="UNSA Logo" />
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
                            Course Enrollment
                        </NavLink>
                    </CNavItem>
                    <CNavItem>
                        <NavLink to="/student/student-enrollment" className="nav-link">
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
                <button
                    type="button"
                    className="sidebar-toggle-btn"
                    onClick={() => setSidebarOpen((prev) => !prev)}
                >
                    <CIcon icon={cilMenu} />
                </button>
                <CContainer fluid className="portal-body">
                    <Outlet />
                </CContainer>
            </div>
        </div>
    );
}
