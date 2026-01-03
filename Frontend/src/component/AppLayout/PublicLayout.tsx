// layouts/PublicLayout.tsx
import { Outlet } from "react-router-dom";

const PublicLayout = () => (
  <div style={{
    minHeight: "100vh",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f5f5f5",
  }}>
    <Outlet />
  </div>
);

export default PublicLayout;
