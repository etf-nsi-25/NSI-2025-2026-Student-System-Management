import "./GradeManagementPage.css";

import { useEffect, useState } from "react";
import CourseSelector from "../../component/teacher/CourseSelector";
import GradesTable from "../../component/teacher/GradesTable";
import GradeDistribution from "../../component/teacher/GradeDistribution";
import PerformanceSummary from "../../component/teacher/PerformanceSummary";

import { getProfessorContextMock } from "../../service/teacher/professorContext.mock";
import { getProfessorGrades, saveProfessorGrades } from "../../service/teacher/professorGrades.api";

import type { StudentGradeListResponse } from "../../dto/GradeDTO";

export default function ProfessorGradesPage() {
    // Context / dropdown data 
    const [context, setContext] = useState<any>(null);
    useEffect(() => {
        getProfessorContextMock().then(setContext);
    }, []);

    // Selected semester/course/exam 
    const [selectedSemester, setSelectedSemester] = useState("");
    const [selectedCourse, setSelectedCourse] = useState<number | null>(null);
    const [selectedExam, setSelectedExam] = useState<number | null>(null);

    // Grades state
    const [gradesData, setGradesData] = useState<StudentGradeListResponse | null>(null);
    const [editedGrades, setEditedGrades] = useState<Record<number, number | null>>({});

    // Load grades for selected exam
    const loadGrades = async (examId: number) => {
        const data = await getProfessorGrades(examId);
        setGradesData(data);
        setEditedGrades({});
    };


    // Handle grade changes
    const handleGradeChange = (studentId: number, points: number | null) => {
        setEditedGrades(prev => ({ ...prev, [studentId]: points }));

        setGradesData(prev => {
            if (!prev) return prev;
            const updatedGrades = prev.grades.map(g =>
                g.studentId === studentId
                    ? { ...g, points, passed: points !== null ? points >= 50 : null }
                    : g
            );
            return { ...prev, grades: updatedGrades };
        });
    };

    // Save changes
    const handleSave = async () => {
        if (!gradesData) return;

        const changedRows = Object.entries(editedGrades).map(([id, points]) => ({
            studentId: Number(id),
            points
        }));

        try {
            await saveProfessorGrades(gradesData.examId, changedRows);
            alert("Grades saved successfully");
            setEditedGrades({});
        } catch (err: any) {
            alert("Failed to save grades: " + err.message);
        }
    };

    // Initialize dropdowns if not already set
    useEffect(() => {
        if (!context) return; // guard inside useEffect
    
        if (!selectedSemester) setSelectedSemester(context.semesters[0]);
        if (!selectedCourse && context.courses.length > 0) setSelectedCourse(context.courses[0].id);
    }, [context, selectedSemester, selectedCourse]);

    if (!context) return <p>Loading context...</p>;

    return (
        <div className="grades-page">
            {/* TOP CARD: Course selector */}
            <div className="card">
                <h2>Course Grades</h2>
                <CourseSelector
                    semesters={context.semesters}
                    courses={context.courses}
                    exams={context.exams}
                    selectedSemester={selectedSemester}
                    selectedCourse={selectedCourse || 0}
                    selectedExam={selectedExam}
                    onSemesterChange={setSelectedSemester}
                    onCourseChange={setSelectedCourse}
                    onExamChange={setSelectedExam}
                    onLoadGrades={() => selectedExam && loadGrades(selectedExam)}
                />
            </div>

            {/* Grades table */}
            <div className="card">
                <h2>Exam results</h2>
                {gradesData ? (
                    <GradesTable grades={gradesData.grades} onGradeChange={handleGradeChange} />
                ) : (
                    <p>Select a course and exam to load grades</p>
                )}
            </div>

            {/* Bottom grid: chart + performance summary */}
            {gradesData && (
                <div className="bottom-grid">
                    <div className="card">
                        <GradeDistribution grades={gradesData.grades} />
                    </div>

                    <div className="right-column">
                        <div className="card">
                            <PerformanceSummary grades={gradesData.grades} />
                        </div>
                        <div className="card">
                            {/* Optional: duplicate summary */}
                            <PerformanceSummary grades={gradesData.grades} />
                        </div>
                    </div>
                </div>
            )}

            {/* Save button */}
            {gradesData && (
                <div className="card table-actions">
                    <button
                        className="primary"
                        onClick={handleSave}
                        disabled={Object.keys(editedGrades).length === 0}
                    >
                        Save changes
                    </button>
                </div>
            )}
        </div>
    );
}
