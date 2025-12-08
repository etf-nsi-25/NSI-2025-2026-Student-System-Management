
import React from 'react';
import { Route, Routes } from 'react-router';
import { Home } from '../page/home/home';
import CourseListPage from '../page/university/courses/CourseListPage';
import TwoFASetupPage from '../page/identity/2FASetupPage';
import { Login } from '../page/login/login.tsx';
import { ProtectedRoute } from '../component/ProtectedRoute.tsx';
import { DocumentCenter,  ProfileSettings, RequestManagement, StudentAnalytics, StudentLayout, StudentSupport } from '../features/student/index.ts';
import EnrollmentPage from "../page/enrollment/enrollment.tsx";
import DashboardPage from '../page/student dashboard/dashboard.tsx';

export function Router(): React.ReactNode {
  return (
    <Routes>
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

      {/* Student Portal Routes - All under /student with Layout */}
      <Route path="/student" element={
        <ProtectedRoute>
          <CourseListPage />
        </ProtectedRoute>
      } />
      {/* Student Portal Routes - All under /student with Layout */}
      <Route path="/student" element={
        <ProtectedRoute>
          <StudentLayout />
        </ProtectedRoute>
      }>        {/*StudentLayout */}
        <Route path="dashboard" element={<DashboardPage/>} />
        <Route path="document-center" element={<DocumentCenter />} />
        <Route path="analytics" element={<StudentAnalytics />} />
        <Route path="request-management" element={<RequestManagement />} />
        <Route path="enrollment" element={<EnrollmentPage />} />
        <Route path="profile-settings" element={<ProfileSettings />} />
        <Route path="support" element={<StudentSupport />} />
        <Route index element={<DashboardPage />} /> {/* Default to dashboard */}
      </Route>
      {/* Unauthorized Page */}
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

      <Route
        path="/faculty/courses"
        element={
          <ProtectedRoute>
            <CourseListPage />
          </ProtectedRoute>
        }
      />

    </Routes>
  );
}
