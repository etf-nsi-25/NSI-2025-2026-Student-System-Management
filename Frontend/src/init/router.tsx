import React from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';

//////////// FEATURE / PBI_258 ////////////
import UserManagementPage from '../page/user-management/UserManagementPage';
import DashboardPage from '../page/dashboard/DashboardPage';
import CourseManagementPage from '../page/course-management/CourseManagementPage';
import TenantManagementPage from '../page/tenant-management/TenantManagementPage';
import { ProfileSettings as TeacherProfileSettings } from '../features/teacher';
import { ProfessorSupport } from '../features/teacher';
//////////// MASTER ////////////
import CourseListPage from '../page/university/courses/CourseListPage';
import TwoFASetupPage from "../page/identity/2FASetupPage";
import { Login } from '../page/login/login.tsx';
import { ProtectedRoute } from '../component/ProtectedRoute.tsx';
import { DocumentCenter, ProfileSettings, StudentAnalytics, StudentLayout, StudentSupport } from '../features/student/index.ts';
import EnrollmentPage from "../page/enrollment/enrollment.tsx";
import { EnrollmentStudentPage } from '../page/enrollment/enrollmentPage.tsx';
import StudentDashboardPage from '../page/student dashboard/dashboard.tsx';
import DocumentCenterDashboard from '../page/document-center/documentCenter.tsx';
import UniversityDashboard from '../page/university-dashboard/UniversityDashboard';
import { TeacherLayout } from '../features/teacher';
import { AssistentLayout } from '../features/assistent';
import { AssistentProfileSettings } from '../page/profile/AssistentProfileSettings';
import { AssistentSupport } from '../page/support/AssistentSupport';
import { AdminLayout } from '../features/admin';
import { SuperadminLayout } from '../features/superadmin';
import ProfessorDashboardPage from '../page/professor-dashboard/dashboard';
import { FacultyProfileSettings } from '../features/admin';
import UniversityProfileSettings from '../page/profile/UniversityProfileSettings';
// layouts
import PublicLayout from '../component/AppLayout/PublicLayout';
import AssistantDashboardPage from '../page/assistant-dashboard/dashboard';
import RequestManagement from '../page/requests/RequestManagement';
import AcademicRecordsPage from '../page/academic-records/AcademicRecordsPage.tsx';


export function Router(): React.ReactNode {
  return (
    <Routes>

            {/* ================= PUBLIC ROUTES ================= */}
      <Route element={<PublicLayout />}>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route
          path="/unauthorized"
          element={
            <div
              className="d-flex flex-column justify-content-center align-items-center"
              style={{ height: '100vh' }}
            >
              <h1>403 - Unauthorized</h1>
              <p>You do not have permission to access this page.</p>
              <a href="/login" className="btn btn-primary">
                Return to Login
              </a>
            </div>
          }
        />
      </Route>

      {/* ================= PROTECTED ROUTES ================= */}
      <Route element={<ProtectedRoute />}>

        

        {/* ===== STUDENT AREA ===== */}
        <Route path="/student" element={<StudentLayout />}>
          <Route index element={<StudentDashboardPage />} />
          <Route path="dashboard" element={<StudentDashboardPage />} />
          <Route path="document-center" element={<DocumentCenter />} />
          <Route path="analytics" element={<StudentAnalytics />} />
          <Route path="request-management" element={<RequestManagement />} />
          <Route path="enrollment" element={<EnrollmentPage />} />
          <Route path="profile-settings" element={<ProfileSettings />} />
          <Route path="support" element={<StudentSupport />} />
          <Route path="student-enrollment" element={<EnrollmentStudentPage />} />
          <Route path="academic-records" element={<AcademicRecordsPage />} />
        </Route>        

          
        {/* ===== TEACHER AREA ===== */}
        <Route path="/teacher" element={<TeacherLayout />}>
          <Route index element={<ProfessorDashboardPage />} />
          <Route path="dashboard" element={<ProfessorDashboardPage />} />
          <Route path="document-center" element={<DocumentCenter />} />
          <Route path="analytics" element={<StudentAnalytics />} />
          <Route path="request-management" element={<RequestManagement />} />
          <Route path="profile-settings" element={<TeacherProfileSettings />} />
          <Route path="support" element={<ProfessorSupport />} />
        </Route>

        {/* ===== ASSISTANT  AREA ===== */}
        <Route path="/assistant" element={<AssistentLayout />}>
          <Route index element={<AssistantDashboardPage />} />
          <Route path="dashboard" element={<AssistantDashboardPage />} />
          <Route path="document-center" element={<DocumentCenter />} />
          <Route path="analytics" element={<StudentAnalytics />} />
          <Route path="request-management" element={<RequestManagement />} />
          <Route path="profile-settings" element={<AssistentProfileSettings />} />
          <Route path="support" element={<AssistentSupport />} />
        </Route>

         {/* ===== ADMIN AREA ===== */}
        <Route path="/admin" element={<AdminLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="dashboard" element={<DashboardPage />} />
          <Route path="user-management" element={<UserManagementPage />} />
          <Route path="course-management" element={<CourseManagementPage />} />
          <Route path="profile-settings" element={<FacultyProfileSettings />} />
        </Route>

          {/* ===== SUPERADMIN AREA ===== */}
        <Route path="/superadmin" element={<SuperadminLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="dashboard" element={<UniversityDashboard />} />
          <Route path="user-management" element={<UserManagementPage />} />
          <Route path="course-management" element={<CourseManagementPage />} />
          <Route path="tenant-management" element={<TenantManagementPage />} />
          <Route path="profile-settings" element={<UniversityProfileSettings />} />
        </Route>

        {/* ===== OTHER PROTECTED ROUTES ===== */}
        <Route path="/2fa/setup" element={<TwoFASetupPage />} />
        <Route path="/document-center" element={<DocumentCenterDashboard />} />
        <Route path="/faculty/courses" element={<CourseListPage />} />
      </Route>

      {/* ================= 404 ================= */}
      <Route
        path="*"
        element={
          <div
            className="d-flex flex-column justify-content-center align-items-center"
            style={{ height: '100vh' }}
          >
            <h1>404 - Not Found</h1>
            <p>The page you are looking for does not exist.</p>
            <a href="/login" className="btn btn-primary">
              Return to Login
            </a>
          </div>
        }
      />

    </Routes>
  );
}