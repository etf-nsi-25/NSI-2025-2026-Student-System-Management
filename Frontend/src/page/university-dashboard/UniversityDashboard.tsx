import React from "react";
import { CCard, CCardBody } from "@coreui/react";
import CIcon from "@coreui/icons-react";
import { cilPeople, cilUser, cilBook, cilChart, } from "@coreui/icons";


const cardStyle: React.CSSProperties = {
  backgroundColor: "#ffffff",
  borderRadius: "16px",
  height: "190px",                   // VEĆE kartice
  padding: "30px 34px",
  boxShadow: "0 8px 20px rgba(0,0,0,0.08)",
  display: "flex",
  alignItems: "center",
};


const subtitleStyle: React.CSSProperties = {
  fontSize: "20px",
  color: "#000000ff", // siva kao na slici
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
   <CCard style={cardStyle}>
  <CCardBody
    className="p-0"
    style={{
      display: "flex",
      alignItems: "center",
      gap: "20px",
    
    }}
  >
    {/* ICON LEFT */}
    <CIcon icon={cilPeople} size="3xl" style={{ color: "#000" }} />

    {/* TEXT RIGHT */}
    <div>
      <div style={{ fontSize: "20px", color: "#000", marginBottom: "30px" }}>
        Students
      </div>

      <div style={{ fontSize: "28px", fontWeight: 700, color: "#000"}}>
        1,052
      </div>

      <div style={subtitleStyle}>
        Total students
      </div>
    </div>
  </CCardBody>
</CCard>



    <CCard style={cardStyle}>
  <CCardBody
    className="p-0"
    style={{
      display: "flex",
      alignItems: "center",
      gap: "20px",
    }}
  >
    {/* ICON LEFT */}
    <CIcon icon={cilUser} size="3xl" style={{ color: "#000" }} />

    {/* TEXT RIGHT */}
    <div>
      <div style={{ fontSize: "20px", color: "#000", marginBottom: "30px" }}>
        Employees
      </div>

      <div style={{ fontSize: "28px", fontWeight: 700, color: "#000" }}>
        121
      </div>

      <div style={subtitleStyle}>
        Total employees
      </div>
    </div>
  </CCardBody>
</CCard>


<CCard style={cardStyle}>
  <CCardBody
    className="p-0"
    style={{
      display: "flex",
      alignItems: "center",
      gap: "20px",
    }}
  >
    {/* ICON LEFT */}
    <CIcon icon={cilBook} size="3xl" style={{ color: "#000" }} />

    {/* TEXT RIGHT */}
    <div>
      <div style={{ fontSize: "20px", color: "#000", marginBottom: "30px" }}>
        Courses
      </div>

      <div style={{ fontSize: "28px", fontWeight: 700, color: "#000" }}>
        86
      </div>

      <div style={subtitleStyle}>
        Total courses
      </div>
    </div>
  </CCardBody>
</CCard>



     <CCard style={cardStyle}>
  <CCardBody
    className="p-0"
    style={{
      display: "flex",
      alignItems: "center",
      gap: "20px",
    }}
  >
    {/* ICON LEFT */}
    <CIcon icon={cilChart} size="3xl" style={{ color: "#000" }} />

    {/* TEXT RIGHT */}
    <div>
      <div style={{ fontSize: "20px", color: "#000", marginBottom: "30px" }}>
        Activity
      </div>

      <div style={{ fontSize: "28px", fontWeight: 700, color: "#000" }}>
        72%
      </div>

      <div style={subtitleStyle}>
        Active users
      </div>
    </div>
  </CCardBody>
</CCard>


    </div>
  );
};

export default UniversityDashboard;
