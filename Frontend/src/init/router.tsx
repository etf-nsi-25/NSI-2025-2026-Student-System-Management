import React from 'react';
import { Route, Routes } from 'react-router';

//////////// VERSION FROM feature/PBI_258 ////////////
import { Home } from '../page/home/home.tsx';
import { Page1 } from '../page/page1/page1.tsx';
import UserManagementPage from '../page/user-management/UserManagementPage.tsx';
import DashboardPage from '../page/dashboard/DashboardPage.tsx';
import CourseManagementPage from '../page/course-management/CourseManagementPage.tsx';
import TenantManagementPage from '../page/tenant-management/TenantManagementPage.tsx';
import StudentSupportPage from '../page/student-support/StudentSupportPage.tsx';
import SettingsPage from '../page/settings/SettingsPage.tsx';
import HelpPage from '../page/help/HelpPage.tsx';

//////////// VERSION FROM master ////////////
import CourseListPage from '../page/university/courses/CourseListPage';
import TwoFASetupPage from "../page/identity/2FASetupPage";
import { Login } from '../page/login/login.tsx';
import { ProtectedRoute } from '../component/ProtectedRoute.tsx';
import { DocumentCenter, ProfileSettings, RequestManagement, StudentAnalytics, StudentLayout, StudentSupport } from '../features/student/index.ts';
import EnrollmentPage from "../page/enrollment/enrollment.tsx";
import StudentDashboardPage from '../page/student dashboard/dashboard.tsx';
import DocumentCenterDashboard from '../page/document-center/documentCenter.tsx';

export function Router(): React.ReactNode {
  return (
    <Routes>

      {/* master routes */}
      <Route path="/login" element={<Login />} />

      <Route path="/" element={
        <ProtectedRoute>
          <Home />
        </ProtectedRoute>
      } />

      <Route path="/2fa/setup" element={
        <ProtectedRoute>
          <TwoFASetupPage />
        </ProtectedRoute>
      } />

      {/* Student area */}
      <Route path="/student" element={
        <ProtectedRoute>
          <StudentLayout />
        </ProtectedRoute>
      }>
        <Route path="dashboard" element={<StudentDashboardPage />} />
        <Route path="document-center" element={<DocumentCenter />} />
        <Route path="analytics" element={<StudentAnalytics />} />
        <Route path="request-management" element={<RequestManagement />} />
        <Route path="enrollment" element={<EnrollmentPage />} />
        <Route path="profile-settings" element={<ProfileSettings />} />
        <Route path="support" element={<StudentSupport />} />
        <Route index element={<StudentDashboardPage />} />
      </Route>

      <Route path="/document-center" element={
        <ProtectedRoute>
          <DocumentCenterDashboard />
        </ProtectedRoute>
      } />

      {/* feature/PBI_258 routes */}
      <Route path="/page1" element={<Page1 />} />
      <Route path="/users" element={<UserManagementPage />} />
      <Route path="/dashboard" element={<DashboardPage />} />
      <Route path="/course-management" element={<CourseManagementPage />} />
      <Route path="/tenant-management" element={<TenantManagementPage />} />
      <Route path="/student-support" element={<StudentSupportPage />} />
      <Route path="/settings" element={<SettingsPage />} />
      <Route path="/help" element={<HelpPage />} />

      {/* error pages */}
      <Route path="/unauthorized" element={
        <div className="d-flex flex-column justify-content-center align-items-center" style={{ height: '100vh' }}>
          <h1>403 - Unauthorized</h1>
          <p>You do not have permission to access this page.</p>
          <a href="/login" className="btn btn-primary">Return to Login</a>
        </div>
      } />

      <Route path="*" element={
        <div className="d-flex flex-column justify-content-center align-items-center" style={{ height: '100vh' }}>
          <h1>404 - Not Found</h1>
          <p>The page you are looking for does not exist.</p>
          <a href="/login" className="btn btn-primary">Return to Login</a>
        </div>
      } />

      <Route path="/faculty/courses" element={
        <ProtectedRoute>
          <CourseListPage />
        </ProtectedRoute>
      } />

    </Routes>
  );
}
