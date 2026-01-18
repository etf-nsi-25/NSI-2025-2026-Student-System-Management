import React from 'react';
import { Route, Routes, Outlet } from 'react-router';
import { Home } from '../page/home/home.tsx';

import UserManagementPage from '../page/user-management/UserManagementPage.tsx';
import DashboardPage from '../page/dashboard/DashboardPage.tsx';
import StudentSupportPage from '../page/student-support/StudentSupportPage.tsx';
import HelpPage from '../page/help/HelpPage.tsx';
import AttendancePage from '../page/attendance/AttendancePage.tsx';
import CourseListPage from '../page/university/courses/CourseListPage';
import { FacultyListingPage } from '../page/faculty-management/FacultyListingPage';
import TwoFASetupPage from "../page/identity/2FASetupPage";
import TwoFAVerifyLoginPage from '../page/identity/2FAVerifyLoginPage';
import { Login } from '../page/login/login.tsx';
import { ProtectedRoute } from '../component/ProtectedRoute.tsx';
import AvailableExamsPage from '../page/university/exams/ExamRegistrationPage.tsx';
import { DocumentCenter, ProfileSettings, StudentAnalytics, StudentSupport } from '../features/student/index.ts';
import EnrollmentPage from "../page/enrollment/enrollment.tsx";
import { EnrollmentStudentPage } from '../page/enrollment/enrollmentPage.tsx';
import StudentDashboardPage from '../page/student dashboard/dashboard.tsx';
import DocumentCenterDashboard from '../page/document-center/documentCenter.tsx';
import AppLayout from '../component/AppLayout/AppLayout.tsx';
import UniversityDashboard from "../page/university-dashboard/UniversityDashboard.tsx";
import { ExamPage } from '../page/exams/ExamPage.tsx';
import { CreateExamPage } from '../page/exams/CreateExamPage.tsx';
import { EditExamPage } from '../page/exams/EditExamPage.tsx'; import RequestManagement from '../page/requests/RequestManagement';
import AcademicRecordsPage from '../page/academic-records/AcademicRecordsPage.tsx';
import CourseOverviewDashboard from '../page/course-overview-dashboard/CourseOverviewDashboard.tsx';
import AssignmentsPage from '../page/assignments/AssignmentsPage.tsx';






export function Router(): React.ReactNode {
  return (
    <Routes>

      {/* master routes */}
      <Route path="/login" element={<Login />} />
      <Route path="/2fa/verify" element={<TwoFAVerifyLoginPage />} />

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
          <AppLayout><Outlet /></AppLayout>
        </ProtectedRoute>
      }>
        <Route path="dashboard" element={<StudentDashboardPage />} />
        <Route path="document-center" element={<DocumentCenterDashboard />} />
        <Route path="analytics" element={<StudentAnalytics />} />
        <Route path="request-management" element={<RequestManagement />} />
        <Route path="exams" element={<AvailableExamsPage />} />
        <Route path="enrollment" element={<EnrollmentPage />} />
        <Route path="profile-settings" element={<ProfileSettings />} />
        <Route path="assignments" element={<AssignmentsPage />} />
        <Route path="support" element={<StudentSupportPage />} />
        <Route path="student-enrollment" element={<EnrollmentStudentPage />} />
        <Route index element={<StudentDashboardPage />} />
        <Route path="academic-records" element={<AcademicRecordsPage />} />
      </Route>

      <Route path="/document-center" element={
        <ProtectedRoute>
          <DocumentCenterDashboard />
        </ProtectedRoute>
      } />

      {/* feature/PBI_258 routes */}
      <Route path="/users" element={<AppLayout><UserManagementPage /></AppLayout>} />
      <Route path="/dashboard" element={<AppLayout><DashboardPage /></AppLayout>} />
      <Route path="/admin/dashboard" element={<AppLayout><DashboardPage /></AppLayout>} />
      <Route path="/teacher/dashboard" element={
        <ProtectedRoute allowedRoles={["teacher"]}>
          <AppLayout><DashboardPage /></AppLayout>
        </ProtectedRoute>
      } />
      <Route path="/assistant/dashboard" element={<AppLayout><DashboardPage /></AppLayout>} />
      <Route path="/course-management" element={
        <ProtectedRoute allowedRoles={['Admin', 'Superadmin', 'Teacher', 'Assistant']}>
          <AppLayout><CourseListPage /></AppLayout>
        </ProtectedRoute>
      } />
      <Route path="/tenant-management" element={
        <ProtectedRoute allowedRoles={['Admin', 'Superadmin']}>
          <AppLayout><FacultyListingPage /></AppLayout>
        </ProtectedRoute>
      } />
      <Route path="/student-support" element={<AppLayout><StudentSupportPage /></AppLayout>} />
      <Route path="/profile-settings" element={<AppLayout><ProfileSettings /></AppLayout>} />
      <Route path="/settings" element={<AppLayout><ProfileSettings /></AppLayout>} />
      <Route path="/attendance" element={<AppLayout><AttendancePage /></AppLayout>} />

      {/*University dashboard*/}

      <Route path="/university-dashboard" element={<AppLayout><UniversityDashboard /></AppLayout>} />
      <Route path="/documents" element={<AppLayout><DocumentCenter /></AppLayout>} />
      <Route path="/analytics" element={<AppLayout><StudentAnalytics /></AppLayout>} />
      <Route path="/requests" element={<AppLayout><RequestManagement /></AppLayout>} />
      <Route path="/profile" element={<AppLayout><ProfileSettings /></AppLayout>} />
      <Route path="/support" element={<AppLayout><StudentSupport /></AppLayout>} />
      <Route path="/help" element={<AppLayout><HelpPage /></AppLayout>} />

      <Route path="/faculty/exams" element={<AppLayout><ExamPage /></AppLayout>} />
      <Route path="/faculty/exams/create" element={<AppLayout><CreateExamPage /></AppLayout>} />
      <Route path="/faculty/exams/:id/edit" element={<AppLayout><EditExamPage /></AppLayout>} />

      <Route path="/student/request-management" element={
        <ProtectedRoute>
          <RequestManagement />
        </ProtectedRoute>
      } />

      <Route path="/faculty/request-management" element={
        <RequestManagement />
      } />

      <Route path="/course-overview-dashboard" element={<AppLayout><CourseOverviewDashboard /></AppLayout>} />

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