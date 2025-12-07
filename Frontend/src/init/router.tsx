import React from "react";
import { Route, Routes } from "react-router";
import { Page1 } from "../page/page1/page1.tsx";
import TwoFASetupPage from "../page/identity/2FASetupPage";
import DashboardPage from "../page/student dashboard/page.tsx";

export function Router(): React.ReactNode {
  return (
    <Routes>
      <Route path="/dashboard" element={<DashboardPage />} />
      <Route path="/page1" element={<Page1 />} />
      <Route path="/2fa/setup" element={<TwoFASetupPage />} />
    </Routes>
  );
}
