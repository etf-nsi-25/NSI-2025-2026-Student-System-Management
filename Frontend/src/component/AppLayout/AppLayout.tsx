import React, { useState, type PropsWithChildren } from "react";
import logo from "../../assets/logo-unsa-sms.png";
import { useNavigate, NavLink } from "react-router-dom";
import { useAuthContext } from "../../init/auth";
import { UserRole } from "../../constants/roles";
import { logoutFromServer, resetAuthInfo } from "../../utils/authUtils";
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
  cilHouse,
  cilChartLine,
  cilClipboard,
  cilCalendar,
  cilEducation,
  cilUser,
  cilLifeRing,
  cilAccountLogout,
  cilPeople,
  cilMenu
} from '@coreui/icons';
import './AppLayout.css';


type SidebarItem =
  | { type: 'item'; label: string; path: string; icon?: any }
  | { type: 'title'; label: string };

const AppLayout: React.FC<PropsWithChildren<object>> = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const { authInfo, setAuthInfo } = useAuthContext();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await logoutFromServer();
    } catch (error) {
      console.error("Logout failed on server:", error);
    } finally {
      resetAuthInfo(setAuthInfo);
      navigate("/login");
    }
  };

  // Role-based sidebar items
  const getSidebarItems = (): SidebarItem[] => {
    if (!authInfo) return [];

    const role = authInfo.role;

    switch (role) {
      case UserRole.Student:
        return [
          { type: 'title', label: 'General' },
          { type: 'item', label: "Student dashboard", path: "/student/dashboard", icon: cilSpeedometer },
          { type: 'item', label: "Document center", path: "/student/document-center", icon: cilFile },
          { type: 'item', label: "Student analytics", path: "/student/analytics", icon: cilChartLine },
          { type: 'item', label: "Request management", path: "/student/request-management", icon: cilClipboard },
          { type: 'item', label: "Course Enrollment", path: "/student/enrollment", icon: cilEducation },
          { type: 'item', label: "Enrollment", path: "/student/student-enrollment", icon: cilEducation },
          { type: 'item', label: "Exams registration", path: "/student/exams", icon: cilCalendar },
          { type: 'title', label: 'Settings' },
          { type: 'item', label: "Profile settings", path: "/profile-settings", icon: cilUser },
          { type: 'title', label: 'Help' },
          { type: 'item', label: "Student support", path: "/student/support", icon: cilLifeRing },
        ];
      case UserRole.Teacher:
        return [
          { type: 'title', label: 'General' },
          { type: 'item', label: "Professor Dashboard", path: "/teacher/dashboard", icon: cilSpeedometer },
          { type: 'item', label: "Course Management", path: "/course-management", icon: cilEducation },
          { type: 'item', label: "Attendance", path: "/attendance", icon: cilClipboard },
          { type: 'item', label: "Request Management", path: "/faculty/request-management", icon: cilFile },
          { type: 'title', label: 'Settings' },
          { type: 'item', label: "Profile settings", path: "/profile-settings", icon: cilUser },
          { type: 'title', label: 'Help' },
          { type: 'item', label: "Help", path: "/help", icon: cilLifeRing },
        ];
      case UserRole.Assistant:
        return [
          { type: 'title', label: 'General' },
          { type: 'item', label: "Professor Dashboard", path: "/assistant/dashboard", icon: cilSpeedometer },
          { type: 'item', label: "Course Management", path: "/course-management", icon: cilEducation },
          { type: 'item', label: "Attendance", path: "/attendance", icon: cilClipboard },
          { type: 'item', label: "Request Management", path: "/faculty/request-management", icon: cilFile },
          { type: 'title', label: 'Settings' },
          { type: 'item', label: "Profile settings", path: "/profile-settings", icon: cilUser },
          { type: 'title', label: 'Help' },
          { type: 'item', label: "Help", path: "/help", icon: cilLifeRing },
        ];
      case UserRole.Admin:
        return [
          { type: 'title', label: 'General' },
          { type: 'item', label: "Faculty Dashboard", path: "/admin/dashboard", icon: cilSpeedometer },
          { type: 'item', label: "User Management", path: "/users", icon: cilPeople },
          { type: 'item', label: "Tenant Management", path: "/tenant-management", icon: cilHouse },
          { type: 'item', label: "Course Management", path: "/course-management", icon: cilEducation },
          { type: 'item', label: "Request Management", path: "/faculty/request-management", icon: cilFile },
          { type: 'title', label: 'Settings' },
          { type: 'item', label: "Profile settings", path: "/profile-settings", icon: cilUser },
          { type: 'title', label: 'Help' },
          { type: 'item', label: "Help", path: "/help", icon: cilLifeRing },
        ];
      case UserRole.Superadmin:
        return [
          { type: 'title', label: 'General' },
          { type: 'item', label: "University Dashboard", path: "/admin/dashboard", icon: cilSpeedometer },
          { type: 'item', label: "User Management", path: "/users", icon: cilPeople },
          { type: 'item', label: "Tenant Management", path: "/tenant-management", icon: cilHouse },
          { type: 'item', label: "Course Management", path: "/course-management", icon: cilEducation },
          { type: 'item', label: "Request Management", path: "/faculty/request-management", icon: cilFile },
          { type: 'title', label: 'Settings' },
          { type: 'item', label: "Profile settings", path: "/profile-settings", icon: cilUser },
          { type: 'title', label: 'Help' },
          { type: 'item', label: "Help", path: "/help", icon: cilLifeRing },
        ];
      default:
        return [];
    }
  };

  const sidebarItems = getSidebarItems();

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
          {sidebarItems.map((item, index) => {
            if (item.type === 'title') {
              return <CNavTitle key={index}>{item.label}</CNavTitle>;
            }
            return (
              <CNavItem key={item.path}>
                <NavLink to={item.path} className="nav-link">
                  {item.icon && <CIcon customClassName="nav-icon" icon={item.icon} />}
                  {item.label}
                </NavLink>
              </CNavItem>
            );
          })}

        </CSidebarNav>

        <div className="sidebar-footer">
          <div className="user-profile">
            <CAvatar color="primary" textColor="white" className="user-avatar">
              {authInfo?.fullName?.charAt(0) || 'U'}
            </CAvatar>
            <div className="user-info">
              <span className="user-name">{authInfo?.fullName || authInfo?.role || "User"}</span>
            </div>
            <button
              className="logout-btn"
              onClick={handleLogout}
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
          {children}
        </CContainer>
      </div>
    </div>
  );
};

export default AppLayout;
