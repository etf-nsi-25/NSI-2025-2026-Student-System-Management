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
    cilChartPie,
    cilAccountLogout,
    cilMenu,
    cilContact,
    cilUser,
    cilMonitor,
} from '@coreui/icons';
import { useAuthContext } from '../../../init/auth';
import logo from '../../../assets/images/login/unsa-sms-logo.png';
import './AdminLayout.css';
import './layout-common.css';
import { useCallback, useState, useEffect } from 'react';
import { logoutFromServer, resetAuthInfo } from '../../../utils/authUtils';


export function AdminLayout() {
    const { authInfo, setAuthInfo } = useAuthContext();
    const [isCollapsed, setIsCollapsed] = useState(false);
    const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
    const [isTablet, setIsTablet] = useState(window.innerWidth >= 768 && window.innerWidth <= 991.98);
    const [sidebarOpen, setSidebarOpen] = useState(false); // mobile only
    const navigate = useNavigate();

    // Handle window resize for mobile/desktop transitions
    useEffect(() => {
        const handleResize = () => {
            const width = window.innerWidth;
            const mobile = width < 768;
            const tablet = width >= 768 && width <= 991.98;
            
            setIsMobile(mobile);
            setIsTablet(tablet);
            
            if (width > 991.98) {
                setSidebarOpen(false);
            }
        };

        window.addEventListener('resize', handleResize);
        handleResize(); // Initial check
        return () => window.removeEventListener('resize', handleResize);
    }, []);


    const logout = useCallback(async () => {

        try {
            await logoutFromServer();
        } catch (err) {
            console.warn("Server logout failed, clearing client session anyway");
        }

        resetAuthInfo(setAuthInfo);

        navigate('/login');
    }, [navigate, setAuthInfo]);

    const handleNavClick = () => {
        if (isMobile || isTablet) {
            setSidebarOpen(false);
        }
    };

    const handleToggleSidebar = () => {
        if (isMobile || isTablet) {
            setSidebarOpen(!sidebarOpen);
        } else {
            setIsCollapsed(!isCollapsed);
        }
    };

    return (
        <div className="student-portal-wrapper">
            {/* Header */}
            <div className="assistant-header">
                <div className="header-left">
                    <img src={logo} alt="UNSA Logo" className="header-logo" />
                    <button
                        className="sidebar-toggle-btn"
                        onClick={handleToggleSidebar}
                    >
                        <CIcon icon={cilMenu} />
                    </button>
                </div>
                <div className="header-title">
                    <span>Faculty Admin Portal</span>
                </div>
                <div 
                    className="header-user"
                    data-initial={authInfo?.fullName ? authInfo.fullName.charAt(0) : 'A'}
                >
                    <span className="user-name">{authInfo?.fullName || 'Admin'}</span>
                </div>
            </div>

            {/* Mobile overlay */}
            {sidebarOpen && isMobile && (
                <div 
                    className="sidebar-overlay"
                    onClick={() => setSidebarOpen(false)}
                />
            )}
            {/* Overlay for mobile/tablet */}
            <div 
                className={`sidebar-overlay ${(isMobile || isTablet) && sidebarOpen ? 'show' : ''}`}
                onClick={() => setSidebarOpen(false)}
            />
            
            <CSidebar
                className={`student-sidebar ${isCollapsed ? 'collapsed' : ''} ${sidebarOpen ? 'show' : ''}`}
                style={{
                    position: 'fixed',
                    zIndex: 1030,
                    top: '60px',
                    bottom: 0,
                    left: 0,
                    width: isCollapsed && !isMobile && !isTablet ? '70px' : '280px',
                    transform: (isMobile || isTablet) && !sidebarOpen ? 'translateX(-100%)' : 'translateX(0)',
                    transition: 'transform 0.3s ease, width 0.3s ease',
                    backgroundColor: '#133E87',
                    boxShadow: (isMobile || isTablet) && sidebarOpen ? '2px 0 10px rgba(0, 0, 0, 0.1)' : 'none',
                }}
            >
                <CSidebarBrand className="sidebar-brand-wrapper" style={{ height: '60px', justifyContent: isCollapsed && !isMobile ? 'center' : 'flex-start' }}>
                    {!isCollapsed || isMobile ? (
                        <div className="unsa-logo">
                            <img src={logo} alt="UNSA Logo" style={{ maxHeight: '40px' }} />
                        </div>
                    ) : null}
                </CSidebarBrand>

                <CSidebarNav>
                    <CNavTitle style={{ display: isCollapsed && !isMobile ? 'none' : 'block' }}>General</CNavTitle>

                    <CNavItem>
                        <NavLink 
                            to="/admin/dashboard" 
                            className="nav-link"
                            onClick={handleNavClick}
                        >
                            <CIcon customClassName="nav-icon" icon={cilChartPie} />
                            {(!isCollapsed || isMobile) && 'Faculty Dashboard'}
                        </NavLink>
                    </CNavItem>

                    <CNavItem>
                        <NavLink 
                            to="/admin/course-management" 
                            className="nav-link"
                            onClick={handleNavClick}
                        >
                            <CIcon customClassName="nav-icon" icon={cilMonitor} />
                            {(!isCollapsed || isMobile) && 'Course Management'}
                        </NavLink>
                    </CNavItem>

                    <CNavItem>
                        <NavLink 
                            to="/admin/user-management" 
                            className="nav-link"
                            onClick={handleNavClick}
                        >
                            <CIcon customClassName="nav-icon" icon={cilContact} />
                            {(!isCollapsed || isMobile) && 'User Management'}
                        </NavLink>
                    </CNavItem>
                    
                    <CNavTitle style={{ display: isCollapsed && !isMobile ? 'none' : 'block' }}>Settings</CNavTitle>
                    <CNavItem>
                        <NavLink 
                            to="/admin/profile-settings" 
                            className="nav-link"
                            onClick={handleNavClick}
                        >
                            <CIcon customClassName="nav-icon" icon={cilUser} />
                            {(!isCollapsed || isMobile) && 'Faculty Profile Settings'}
                        </NavLink>
                    </CNavItem>
                </CSidebarNav>

                <div className="sidebar-footer">
                    <div className="user-profile" style={{ justifyContent: isCollapsed && !isMobile ? 'center' : 'flex-start' }}>
                        <CAvatar color="primary" textColor="white" className="user-avatar">
                            {authInfo?.fullName?.charAt(0) || 'A'}
                        </CAvatar>
                        {(!isCollapsed || isMobile) && (
                            <div className="user-info">
                                <span className="user-name">{authInfo?.fullName || 'Admin'}</span>
                            </div>
                        )}
                        {(!isCollapsed || isMobile) && (
                            <button
                                className="logout-btn"
                                onClick={logout}
                                aria-label="Logout"
                            >
                                <CIcon icon={cilAccountLogout} size="lg" />
                            </button>
                        )}
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
