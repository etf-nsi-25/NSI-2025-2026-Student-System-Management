import React, { useState, useEffect, type PropsWithChildren } from "react";
import logoImage from "../../assets/logo-unsa-sms.png";
import { Link, useLocation } from "react-router-dom";

const Defaultayout: React.FC<PropsWithChildren<object>> = ({ children }) => {
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

  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth < 768;
      setIsMobile(mobile);
      if (!mobile) setSidebarOpen(false);
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

                  <span
                      style={{
                          color: "white",
                          fontSize: "20px",
                          fontWeight: 600,
                          whiteSpace: "nowrap",
                           marginLeft: "25px",
                      }}
                  >
                      University Dashboard
                  </span>

        </div>

        <span style={{ color: "white", fontSize: "16px", fontWeight: 500, paddingRight: "40px" }}>
          Jane Smith
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
          {/* GENERAL */}
          <div style={{ padding: "8px 24px", fontSize: "12px", opacity: 0.7 }}>
            General
          </div>

          <Link to="/university-dashboard" style={getLinkStyle("/university-dashboard")} onClick={handleLinkClick}>
            University dashboard
          </Link>

          <Link to="/documents" style={getLinkStyle("/documents")} onClick={handleLinkClick}>
            Document center
          </Link>

          <Link to="/analytics" style={getLinkStyle("/analytics")} onClick={handleLinkClick}>
            Analytics
          </Link>

          <Link to="/requests" style={getLinkStyle("/requests")} onClick={handleLinkClick}>
            Request management
          </Link>

          {/* SETTINGS */}
          <div style={{ padding: "16px 24px 8px", fontSize: "12px", opacity: 0.7 }}>
            Settings
          </div>

          <Link to="/profile" style={getLinkStyle("/profile")} onClick={handleLinkClick}>
            Profile settings
          </Link>

          {/* HELP */}
          <div style={{ padding: "16px 24px 8px", fontSize: "12px", opacity: 0.7 }}>
            Help
          </div>

          <Link to="/support" style={getLinkStyle("/support")} onClick={handleLinkClick}>
            Support
          </Link>
        </div>

        {/* ===== CONTENT ===== */}
        <div style={{ padding: "24px", width: "100%" }}>
          {children}

        </div>
      </div>
    </div>
  );
};

export default Defaultayout;
