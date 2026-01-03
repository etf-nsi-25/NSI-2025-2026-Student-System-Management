import "./GradeManagementPage.css";
import { useEffect, useState } from "react";

import CourseSelector from "../../component/teacher/CourseSelector";
import GradesTable from "../../component/teacher/GradesTable";
import GradeDistribution from "../../component/teacher/GradeDistribution";
import PerformanceSummary from "../../component/teacher/PerformanceSummary";

import { getProfessorContext } from "../../service/teacher/professorContext.api";
import { getProfessorGrades } from "../../service/teacher/professorGrades.api";
import type { StudentGradeListResponse } from "../../dto/GradeDTO";

export default function ProfessorGradesPage() {
    const [context, setContext] = useState<{
        semesters: string[];
        courses: { id: number; name: string }[];
        exams: { id: number; name: string }[];
    } | null>(null);

    const [gradesData, setGradesData] =
        useState<StudentGradeListResponse | null>(null);

    const [loading, setLoading] = useState(false);

    useEffect(() => {
        getProfessorContext().then(setContext);
    }, []);

    async function loadGrades(examId: number) {
        setLoading(true);
        try {
            const data = await getProfessorGrades(examId);
            setGradesData(data);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="grades-page">
            <div className="card">
                <h2>Course Grades</h2>
                {context && (
                    <CourseSelector
                        semesters={context.semesters}
                        courses={context.courses}
                        onLoadGrades={() => loadGrades(context.exams[0].id)}
                    />
                )}
            </div>

            <div className="card">
                {loading && <p>Loading grades…</p>}
                {gradesData && <GradesTable grades={gradesData.grades} />}
            </div>

            <div className="bottom-grid">
                <div className="card">
                    {gradesData && (
                        <GradeDistribution grades={gradesData.grades} />
                    )}
                </div>

                <div className="right-column">
                    <div className="card">
                        {gradesData && (
                            <PerformanceSummary grades={gradesData.grades} />
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
