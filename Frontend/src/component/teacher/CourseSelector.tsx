import { useEffect } from "react";

type Props = {
    semesters: string[];
    courses: { id: number; name: string; semester: string; exams: number[] }[];
    exams: { id: number; name: string; courseId: number }[];
    selectedSemester: string;
    selectedCourse: number;
    selectedExam: number | null;
    onSemesterChange: (s: string) => void;
    onCourseChange: (id: number) => void;
    onExamChange: (id: number) => void;
    onLoadGrades: () => void;
};

export default function CourseSelector({
    semesters, courses, exams,
    selectedSemester, selectedCourse, selectedExam,
    onSemesterChange, onCourseChange, onExamChange,
    onLoadGrades
}: Props) {
    // Filter exams for selected course
    const filteredExams = exams.filter(e => e.courseId === selectedCourse);

    // If current selectedExam is not in filteredExams, default to first
    useEffect(() => {
        if (!filteredExams.find(e => e.id === selectedExam) && filteredExams.length > 0) {
            onExamChange(filteredExams[0].id);
        }
    }, [selectedCourse, filteredExams]);

    return (
        <div className="course-selector">
            <div>
                <h6>Semester</h6>
                <select value={selectedSemester} onChange={e => onSemesterChange(e.target.value)}>
                    {semesters.map(s => <option key={s}>{s}</option>)}
                </select>
            </div>

            <div>
                <h6>Course</h6>
                <select value={selectedCourse} onChange={e => onCourseChange(Number(e.target.value))}>
                    {courses.filter(c => c.semester === selectedSemester)
                            .map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
            </div>

            <div>
                <h6>Exam</h6>
                <select value={selectedExam ?? undefined} onChange={e => onExamChange(Number(e.target.value))}>
                    {filteredExams.map(e => <option key={e.id} value={e.id}>{e.name}</option>)}
                </select>
            </div>

            <button className="primary" onClick={onLoadGrades}>
                Load grades
            </button>
        </div>
    );
}
