import React from "react";
import type { GradeRow } from "../../types/grades";

type Props = {
  rows: GradeRow[];
  setRows: React.Dispatch<React.SetStateAction<GradeRow[]>>;
};

const GradesTable: React.FC<Props> = ({ rows, setRows }) => {
  const updatePoints = (studentId: number, value: string) => {
    const num = value === "" ? null : Number(value);

    setRows(prev =>
      prev.map(r =>
        r.studentId === studentId
          ? {
              ...r,
              points: num,
              isDirty: true,
              isValid: num === null || (!isNaN(num) && num >= 0 && num <= r.maxPoints),
              passed: num !== null && num >= 50,
              grade: num !== null ? Math.round((num / r.maxPoints) * 10) : null,
            }
          : r
      )
    );
  };

  return (
    <table style={{ width: "100%", borderCollapse: "collapse" }}>
      <thead>
        <tr>
          <th style={thStyle}>Student</th>
          <th style={thStyle}>Points</th>
          <th style={thStyle}>Grade</th>
          <th style={thStyle}>Status</th>
        </tr>
      </thead>
      <tbody>
        {rows.map(row => (
          <tr key={row.studentId}>
            <td style={tdStyle}>{row.name}</td>
            <td style={tdStyle}>
              <input
                type="number"
                value={row.points ?? ""}
                onChange={e => updatePoints(row.studentId, e.target.value)}
                style={{
                  border: row.isValid ? "1px solid #ccc" : "2px solid red",
                  width: "60px",
                }}
              />
            </td>
            <td style={tdStyle}>{row.grade ?? "-"}</td>
            <td style={tdStyle}>
              <span
                style={{
                  color: "white",
                  backgroundColor: row.passed ? "green" : "red",
                  padding: "2px 6px",
                  borderRadius: "4px",
                }}
              >
                {row.passed ? "Pass" : "Fail"}
              </span>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

const thStyle: React.CSSProperties = {
  border: "1px solid #ccc",
  padding: "8px",
  textAlign: "left",
  backgroundColor: "#f4f4f4",
};

const tdStyle: React.CSSProperties = {
  border: "1px solid #ccc",
  padding: "8px",
};

export default GradesTable;
