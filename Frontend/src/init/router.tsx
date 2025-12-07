import React from 'react';
import { Route, Routes } from 'react-router';
import { Home } from '../page/home/home.tsx';
import { Page1 } from '../page/page1/page1.tsx';
import TwoFASetupPage from "../page/identity/2FASetupPage";
import UserManagementPage from '../page/user-management/UserManagementPage.tsx';
import DashboardPage from '../page/dashboard/DashboardPage.tsx';
import CourseManagementPage from '../page/course-management/CourseManagementPage.tsx';
import TenantManagementPage from '../page/tenant-management/TenantManagementPage.tsx';
import StudentSupportPage from '../page/student-support/StudentSupportPage.tsx';
import SettingsPage from '../page/settings/SettingsPage.tsx';
import HelpPage from '../page/help/HelpPage.tsx';

export function Router(): React.ReactNode {
    return (
        <Routes>
            <Route path="/" element={ <Home /> } />
            <Route path="/page1" element={<Page1 />} />
            <Route path="/2fa/setup" element={<TwoFASetupPage />} />
            <Route path="/users" element={<UserManagementPage />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/course-management" element={<CourseManagementPage />} />
            <Route path="/tenant-management" element={<TenantManagementPage />} />   
            <Route path="/student-support" element={<StudentSupportPage />} />    
            <Route path="/settings" element={<SettingsPage />} />   
            <Route path="/help" element={<HelpPage />} />   
        </Routes>
    )
}
