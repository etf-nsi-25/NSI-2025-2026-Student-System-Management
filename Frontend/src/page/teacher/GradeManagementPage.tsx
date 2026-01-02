import React, { useEffect, useState } from "react";
import type { GradeRow } from "../../types/grades";
import type { StudentGradeListResponse } from "../../dto/GradeDTO";
import GradesTable from "../../component/teacher/GradesTable";
import { useAPI } from "../../context/services";


const ProfessorGradesPage: React.FC = () => {
const api = useAPI();   
  const [semester, setSemester] = useState<string>("");
  const [examId, setExamId] = useState<number | "">("");
  const [exams, setExams] = useState<{ id: number; name: string }[]>([]);
  const [rows, setRows] = useState<GradeRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string>("");

  // Load courses/exams when semester changes
  useEffect(() => {
    if (!semester) return;
    // Example: call your existing API to fetch courses/exams per semester
    api.get(`/api/faculty/exams?semester=${semester}`)
      .then(res => setExams(res.data))
      .catch(() => setMessage("Failed to load exams"));
  }, [semester]);

  // Load grades from backend
  const loadGrades = async () => {
    if (!examId) return;
    setLoading(true);
    setMessage("");
    try {
      const res: StudentGradeListResponse = await api.getGrades(examId as number);
      setRows(
        res.grades.map(g => ({
          studentId: g.studentId,
          name: g.studentName,
          points: g.points,
          grade: g.points !== null ? Math.round((g.points / 100) * 10) : null,
          passed: g.passed ?? false,
          maxPoints: 100,
          isDirty: false,
          isValid: true,
        }))
      );
    } catch {
      setMessage("Failed to load grades");
    } finally {
      setLoading(false);
    }
  };

  // Save only changed rows
  const handleSave = async () => {
    const changed = rows
      .filter(r => r.isDirty)
      .map(r => ({ studentId: r.studentId, points: r.points }));

    if (!changed.length) return;

    try {
      await api.saveGrades(examId as number, changed);
      setMessage("Grades saved successfully");
      loadGrades();
    } catch {
      setMessage("Failed to save grades");
    }
  };

  // Disable save if invalid or no changes
  const hasInvalid = rows.some(r => !r.isValid);
  const hasDirty = rows.some(r => r.isDirty);

  return (
    <div style={{ padding: "20px" }}>
      <h2>Professor Grades Management</h2>

      <div style={{ display: "flex", gap: "10px", marginBottom: "10px" }}>
        {/* Semester selection */}
        <select value={semester} onChange={e => setSemester(e.target.value)}>
          <option value="">Select Semester</option>
          <option value="Winter2025">Winter 2025</option>
          <option value="Summer2025">Summer 2025</option>
        </select>

        {/* Exam selection */}
        <select value={examId} onChange={e => setExamId(Number(e.target.value))}>
          <option value="">Select Exam</option>
          {exams.map(ex => (
            <option key={ex.id} value={ex.id}>
              {ex.name}
            </option>
          ))}
        </select>

        {/* Load grades */}
        <button onClick={loadGrades} disabled={!semester || !examId}>
          Load Grades
        </button>

        {/* Save changes */}
        <button onClick={handleSave} disabled={hasInvalid || !hasDirty}>
          Save Changes
        </button>
      </div>

      {message && <p>{message}</p>}
      {loading ? <p>Loading grades...</p> : <GradesTable rows={rows} setRows={setRows} />}
    </div>
  );
};

export default ProfessorGradesPage;
