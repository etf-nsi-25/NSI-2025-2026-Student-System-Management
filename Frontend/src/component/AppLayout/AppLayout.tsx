import React, { useState, useEffect, type PropsWithChildren } from "react";
import logoImage from "../../assets/logo-unsa-sms.png";
import {Link, useLocation} from "react-router-dom";


const AppLayout: React.FC<PropsWithChildren<object>> = ({ children }) => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [sidebarOpen, setSidebarOpen] = useState(false); // mobile only
  const location=useLocation();

  const handleLinkClick = () => {
if (isMobile) {
setSidebarOpen(false);
}
};

 const getLinkStyle = (path: string) => {
  const isActive = location.pathname === path;

  // Kada je sidebar sklopljen (collapsed), ukloni aktivni stil
  if (isCollapsed && !isMobile) {
    return {
      padding: "10px 20px",
      color: "white",
      textDecoration: "none",
      backgroundColor: "transparent",
      borderLeft: "4px solid transparent",
      fontWeight: "normal",
      transition: "0.2s",
    };
  }

  // Normalno stanje (sidebar otvoren)
  return {
    padding: "10px 20px",
    color: "white",
    textDecoration: "none",
    backgroundColor: isActive ? "#005bb5" : "transparent",
    borderLeft: isActive ? "4px solid #ffffff" : "4px solid transparent",
    fontWeight: isActive ? "bold" : "normal",
    transition: "0.2s",
  };
};



  // LISTEN TO WINDOW RESIZE
  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth < 768;
      setIsMobile(mobile);

      if (!mobile) {
        setSidebarOpen(false);
      }
    };

    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const sidebarWidth = isCollapsed ? "70px" : "300px";

  return (
    <div style={{ minHeight: "100vh", backgroundColor: "#d9e7f5", overflowX: "hidden" }}>

      {/* TOP BLUE HEADER */}
      <div
        style={{
          width: "100%",
          height: "60px",
          backgroundColor: "#003b82",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          paddingLeft: "0px",
          paddingRight: "10px",
        }}
      >
        {/* LEFT: Logo + Collapse button */}
        <div style={{ display: "flex", alignItems: "center", gap: "20px" }}>
          <img src={logoImage} alt="logo" style={{ height: "42px", paddingLeft: "50px" }} />

          <button
            onClick={() => {
              if (isMobile) setSidebarOpen(!sidebarOpen);
              else setIsCollapsed(!isCollapsed);
            }}
            style={{
              background: "transparent",
              border: "none",
              color: "white",
              fontSize: "24px",
              cursor: "pointer",
            }}
          >
            ☰
          </button>
        </div>

        <span style={{ color: "white", fontSize: "16px", fontWeight: 500, paddingRight: "40px" }}>
          Faculty admin
        </span>
      </div>


      {/* LAYOUT WRAPPER */}
      <div style={{ display: "flex", flexDirection: "row" }}>

        {/* MOBILE OVERLAY */}
        {isMobile && sidebarOpen && (
          <div
            onClick={() => setSidebarOpen(false)}
            style={{
              position: "fixed",
              inset: 0,
              backgroundColor: "rgba(0,0,0,0.3)",
              zIndex: 9,
            }}
          ></div>
        )}

        {/* SIDEBAR */}
        <div
          style={{
            width: isMobile ? "250px" : sidebarWidth,
            backgroundColor: "#003b82",
            minHeight: "calc(100vh - 60px)",
            paddingTop: "20px",
            color: "white",
            display: "flex",
            flexDirection: "column",
            gap: "10px",
            transition: "all 0.3s",
            overflow: "hidden",
            position: isMobile ? "fixed" : "relative",
            zIndex: 10,
            inset: 0,
            transform: isMobile
              ? sidebarOpen
                ? "translateX(0)"
                : "translateX(-100%)"
              : "none",
          }}
        >
          <Link to="/dashboard" style={getLinkStyle("/dashboard")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Dashboard"}
          </Link>

          <Link to="/course-management" style={getLinkStyle("/course-management")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Course Management"}
          </Link>

          <Link to="/faculty/request-management" style={getLinkStyle("/faculty/request-management")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Request Management"}
          </Link>

          <Link to="/users" style={getLinkStyle("/users")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "User Management"}
          </Link>

          <Link to="/tenant-management" style={getLinkStyle("/tenant-management")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Tenant Management"}
          </Link>

          <Link to="/student-support" style={getLinkStyle("/student-support")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Student Support"}
          </Link>

          <Link to="/settings" style={getLinkStyle("/settings")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Settings"}
          </Link>

          <Link to="/help" style={getLinkStyle("/help")} onClick={handleLinkClick}>
            {(!isCollapsed || isMobile) && "Help"}
          </Link>

        </div>

        {/* CONTENT */}
        <div style={{ padding: "20px 25px 60px 25px", width: "100%" }}>
          {children}

          {/* FOOTER */}
          <footer
            style={{
              marginTop: "40px",
              fontSize: "12px",
              color: "#424242",
              textAlign: "center",
            }}
          >
            Faculty Admin – User Management (reference layout)
          </footer>
        </div>

      </div>
    </div>
  );
};

export default AppLayout;
