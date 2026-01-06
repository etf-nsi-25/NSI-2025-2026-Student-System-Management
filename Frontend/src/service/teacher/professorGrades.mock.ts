import type { StudentGradeListResponse } from '../../dto/GradeDTO';

function delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// Fixed grades for all exams
const gradesByExam: Record<number, { studentId: number; studentName: string; points: number | null; passed: boolean | null; dateRecorded: string | null; url?: string | null }[]> = {
    1: [ // Midterm - Algorithms
        { studentId: 101, studentName: "Amar Hadžić", points: 78, passed: true, dateRecorded: "2025-01-05T10:00:00Z" },
        { studentId: 102, studentName: "Lejla Kovačević", points: 48, passed: false, dateRecorded: "2025-01-05T10:05:00Z" },
        { studentId: 103, studentName: "Haris Mehmedović", points: 65, passed: true, dateRecorded: "2025-01-05T10:10:00Z" },
        { studentId: 104, studentName: "Amina Begić", points: 92, passed: true, dateRecorded: "2025-01-05T10:15:00Z" },
    ],
    2: [ // Final Exam - Algorithms
        { studentId: 101, studentName: "Amar Hadžić", points: 85, passed: true, dateRecorded: "2025-01-10T10:30:00Z" },
        { studentId: 102, studentName: "Lejla Kovačević", points: 48, passed: false, dateRecorded: "2025-01-10T10:32:00Z" },
        { studentId: 103, studentName: "Haris Mehmedović", points: null, passed: null, dateRecorded: null },
        { studentId: 104, studentName: "Amina Begić", points: 92, passed: true, dateRecorded: "2025-01-10T10:35:00Z" },
    ],
    3: [ // Project Presentation - Software Engineering
        { studentId: 201, studentName: "Sara Novak", points: 88, passed: true, dateRecorded: "2025-05-01T11:00:00Z" },
        { studentId: 202, studentName: "Ivan Kovač", points: 70, passed: true, dateRecorded: "2025-05-01T11:05:00Z" },
        { studentId: 203, studentName: "Maja Ilić", points: 45, passed: false, dateRecorded: "2025-05-01T11:10:00Z" },
        { studentId: 204, studentName: "Adnan Hodžić", points: 92, passed: true, dateRecorded: "2025-05-01T11:15:00Z" },
    ],
    4: [ // Midterm - Database Systems
        { studentId: 301, studentName: "Lejla Kovačević", points: 60, passed: true, dateRecorded: "2026-01-10T09:00:00Z" },
        { studentId: 302, studentName: "Haris Mehmedović", points: 72, passed: true, dateRecorded: "2026-01-10T09:05:00Z" },
        { studentId: 303, studentName: "Amar Hadžić", points: 40, passed: false, dateRecorded: "2026-01-10T09:10:00Z" },
        { studentId: 304, studentName: "Amina Begić", points: 95, passed: true, dateRecorded: "2026-01-10T09:15:00Z" },
    ],
};

// Fetch grades for an exam
export async function getProfessorGradesMock(examId: number): Promise<StudentGradeListResponse> {
    await delay(500);
    const grades = gradesByExam[examId] || [];
    return { examId, examName: `Exam ${examId}`, grades };
}

// Save edited grades
export async function saveProfessorGradesMock(
    examId: number,
    grades: { studentId: number; points: number | null }[]
): Promise<void> {
    await delay(500);

    console.log(`Mock save for exam ${examId}:`);
    console.table(grades);

    grades.forEach(g => {
        const existing = gradesByExam[examId].find(s => s.studentId === g.studentId);
        if (existing) {
            existing.points = g.points;
            existing.passed = g.points !== null ? g.points >= 50 : null;
            existing.dateRecorded = new Date().toISOString();
        }
    });

    // Optional: simulate random error
    if (Math.random() < 0.05) {
        throw new Error("Mock API error: Failed to save grades");
    }
}
