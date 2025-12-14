import React from "react";
import { CCard, CCardBody } from "@coreui/react";

const cardStyle: React.CSSProperties = {
  backgroundColor: "#ffffff",
  borderRadius: "16px",
  height: "190px",                   // VEĆE kartice
  padding: "30px 34px",
  boxShadow: "0 8px 20px rgba(0,0,0,0.08)",
  display: "flex",
  alignItems: "center",
};

const titleStyle: React.CSSProperties = {
  fontSize: "30px",                  // veći naslov
  fontWeight: 400,
  color: "#000000",                  // CRNA boja
  marginBottom: "10px",
};


const UniversityDashboard: React.FC = () => {
  return (
    <div
      style={{
        maxWidth: "1020px",
        margin: "120px auto 0 auto",

        display: "grid",
        gridTemplateColumns: "repeat(2, 1fr)",
        gap: "36px",
      }}
    >
      {/* STUDENTS */}
      <CCard style={cardStyle}>
        <CCardBody className="p-0">
          <div style={titleStyle}>Students</div>
        </CCardBody>
      </CCard>

      {/* EMPLOYEES */}
      <CCard style={cardStyle}>
        <CCardBody className="p-0">
          <div style={titleStyle}>Employees</div>
        </CCardBody>
      </CCard>

      {/* COURSES */}
      <CCard style={cardStyle}>
        <CCardBody className="p-0">
          <div style={titleStyle}>Courses</div>
        </CCardBody>
      </CCard>

      {/* ACTIVITY */}
      <CCard style={cardStyle}>
        <CCardBody className="p-0">
          <div style={titleStyle}>Activity</div>
        </CCardBody>
      </CCard>
    </div>
  );
};

export default UniversityDashboard;
