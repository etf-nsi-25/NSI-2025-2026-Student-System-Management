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
import { EnrollmentStudentPage } from '../page/enrollment/enrollmentPage.tsx';
import StudentDashboardPage from '../page/student dashboard/dashboard.tsx';
import DocumentCenterDashboard from '../page/document-center/documentCenter.tsx';
import AppLayout from '../component/AppLayout/AppLayout.tsx';
import DefaultLayout from '../component/UniversityDashboardLayout/DefaultLayout.tsx';
import UniversityDashboard from "../page/university-dashboard/UniversityDashboard.tsx";
import { ExamPage } from '../page/exams/ExamPage.tsx';
import { CreateExamPage } from '../page/exams/CreateExamPage.tsx';
import { EditExamPage } from '../page/exams/EditExamPage.tsx';

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
        <Route path="student-enrollment" element={<EnrollmentStudentPage />} />  
        <Route index element={<StudentDashboardPage />} />
      </Route>

      <Route path="/document-center" element={
        <ProtectedRoute>
          <DocumentCenterDashboard />
        </ProtectedRoute>
      } />

      {/* feature/PBI_258 routes */}
      <Route path="/page1" element={<AppLayout><Page1 /></AppLayout>} />
      <Route path="/users" element={<AppLayout><UserManagementPage /></AppLayout>} />
      <Route path="/dashboard" element={<AppLayout><DashboardPage /></AppLayout>} />
      <Route path="/course-management" element={<AppLayout><CourseManagementPage /></AppLayout>} />
      <Route path="/tenant-management" element={<AppLayout><TenantManagementPage /></AppLayout>} />
      <Route path="/student-support" element={<AppLayout><StudentSupportPage /></AppLayout>} />
      <Route path="/settings" element={<AppLayout><SettingsPage /></AppLayout>} />
      <Route path="/help" element={<AppLayout><HelpPage /></AppLayout>} />

      {/*University dashboard*/}

          <Route path="/university-dashboard" element={<DefaultLayout><UniversityDashboard /></DefaultLayout>} />
          <Route path="/documents" element={<DefaultLayout><DocumentCenter /></DefaultLayout>} />
          <Route path="/analytics" element={<DefaultLayout><StudentAnalytics /></DefaultLayout>} />
          <Route path="/requests" element={<DefaultLayout><RequestManagement /></DefaultLayout>} />
          <Route path="/profile" element={<DefaultLayout><SettingsPage /></DefaultLayout>} />
          <Route path="/support" element={<DefaultLayout><StudentSupport /></DefaultLayout>} />
          <Route path="/help" element={<DefaultLayout><HelpPage /></DefaultLayout>} />

      <Route path="/faculty/exams" element={<DefaultLayout><ExamPage /></DefaultLayout>} />
      <Route path="/faculty/exams/create" element={<DefaultLayout><CreateExamPage /></DefaultLayout>} />
      <Route path="/faculty/exams/:id/edit" element={<DefaultLayout><EditExamPage /></DefaultLayout>} />

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
