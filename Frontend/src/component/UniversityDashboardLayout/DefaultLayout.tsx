import React, { useEffect, useMemo, useState, type PropsWithChildren } from "react";
import { Link, useLocation } from "react-router-dom";
import type { CSSProperties } from "react";
import logoImage from "../../assets/logo-unsa-sms.png";

const DefaultLayout: React.FC<PropsWithChildren<object>> = ({ children }) => {
  const location = useLocation();

  const [isCollapsed, setIsCollapsed] = useState(false);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [sidebarOpen, setSidebarOpen] = useState(false); // mobile only

  const showText = useMemo(() => {
    // On mobile, always show text (the sidebar is a drawer)
    if (isMobile) return true;
    // On desktop, hide text when collapsed
    return !isCollapsed;
  }, [isCollapsed, isMobile]);

  const handleLinkClick = () => {
    if (isMobile) setSidebarOpen(false);
  };

  const getLinkStyle = (path: string): CSSProperties => {
    const isActive = location.pathname === path;

    // When collapsed (desktop), do not show the "active" highlight
    if (!isMobile && isCollapsed) {
      return {
        padding: "10px 20px",
        color: "white",
        textDecoration: "none",
        backgroundColor: "transparent",
        borderLeft: "4px solid transparent",
        fontWeight: 400,
        transition: "0.2s",
        display: "block",
        whiteSpace: "nowrap",
      };
    }

    return {
      padding: "10px 20px",
      color: "white",
      textDecoration: "none",
      backgroundColor: isActive ? "#005bb5" : "transparent",
      borderLeft: isActive ? "4px solid #ffffff" : "4px solid transparent",
      fontWeight: isActive ? 700 : 400,
      transition: "0.2s",
      display: "block",
      whiteSpace: "nowrap",
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

  const sidebarWidth = !isMobile && isCollapsed ? "70px" : isMobile ? "250px" : "300px";

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
          paddingRight: "10px",
        }}
      >
        {/* LEFT: Logo + Collapse button + Title */}
        <div style={{ display: "flex", alignItems: "center", gap: "20px" }}>
          <img src={logoImage} alt="logo" style={{ height: "42px", paddingLeft: "50px" }} />

          <button
            onClick={() => {
              if (isMobile) setSidebarOpen((v) => !v);
              else setIsCollapsed((v) => !v);
            }}
            style={{
              background: "transparent",
              border: "none",
              color: "white",
              fontSize: "24px",
              cursor: "pointer",
            }}
            aria-label="Toggle sidebar"
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
          />
        )}

        {/* SIDEBAR */}
        <div
          style={{
            width: sidebarWidth,
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
            transform: isMobile ? (sidebarOpen ? "translateX(0)" : "translateX(-100%)") : "none",
          }}
        >
          {/* GENERAL */}
          {showText && (
            <div style={{ padding: "8px 24px", fontSize: "12px", opacity: 0.7 }}>
              General
            </div>
          )}

          <Link to="/university-dashboard" style={getLinkStyle("/university-dashboard")} onClick={handleLinkClick}>
            {showText ? "University dashboard" : "\u00A0"}
          </Link>

          <Link to="/documents" style={getLinkStyle("/documents")} onClick={handleLinkClick}>
            {showText ? "Document center" : "\u00A0"}
          </Link>

          <Link to="/analytics" style={getLinkStyle("/analytics")} onClick={handleLinkClick}>
            {showText ? "Analytics" : "\u00A0"}
          </Link>

          <Link to="/requests" style={getLinkStyle("/requests")} onClick={handleLinkClick}>
            {showText ? "Request management" : "\u00A0"}
          </Link>

          {/* SETTINGS */}
          {showText && (
            <div style={{ padding: "16px 24px 8px", fontSize: "12px", opacity: 0.7 }}>
              Settings
            </div>
          )}

          <Link to="/profile" style={getLinkStyle("/profile")} onClick={handleLinkClick}>
            {showText ? "Profile settings" : "\u00A0"}
          </Link>

          {/* HELP */}
          {showText && (
            <div style={{ padding: "16px 24px 8px", fontSize: "12px", opacity: 0.7 }}>
              Help
            </div>
          )}

          <Link to="/support" style={getLinkStyle("/support")} onClick={handleLinkClick}>
            {showText ? "Support" : "\u00A0"}
          </Link>
        </div>

        {/* CONTENT */}
        <div style={{ padding: "24px", width: "100%" }}>{children}</div>
      </div>
    </div>
  );
};

export default DefaultLayout;