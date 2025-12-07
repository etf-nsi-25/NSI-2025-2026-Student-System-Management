import React from "react";
import { Route, Routes } from "react-router";
import { Home } from "../page/home/home";
import { Page1 } from "../page/page1/page1";
import CourseListPage from "../page/university/courses/CourseListPage";
import TwoFASetupPage from "../page/identity/2FASetupPage";
import { Login } from "../page/login/login.tsx";
import { ProtectedRoute } from "../component/ProtectedRoute.tsx";
import EnrollmentPage from "../page/enrollment/enrollment.tsx";
import DashboardPage from "../page/student dashboard/page.tsx";

export function Router(): React.ReactNode {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Home />
          </ProtectedRoute>
        }
      />
      <Route
        path="/page1"
        element={
          <ProtectedRoute>
            <Page1 />
          </ProtectedRoute>
        }
      />
      <Route
        path="/enrollment"
        element={
          <ProtectedRoute>
            <EnrollmentPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/2fa/setup"
        element={
          <ProtectedRoute>
            <TwoFASetupPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/faculty/courses"
        element={
          <ProtectedRoute>
            <CourseListPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        }
      />
    </Routes>
  );
}
