import type { StudentGradeListResponse } from '../../dto/GradeDTO';

function delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// Sample students pool
const students = [
    "Amar Hadžić",
    "Lejla Kovačević",
    "Haris Mehmedović",
    "Amina Begić",
    "Nina Petrović",
    "Marko Jurić",
    "Sara Novak",
    "Ivan Kovač",
    "Maja Ilić",
    "Adnan Hodžić"
];

export async function getProfessorGradesMock(
    examId: number
): Promise<StudentGradeListResponse> {
    await delay(1000);

    const grades = students.map((name, idx) => {
        // Randomize points, some nulls
        const points = Math.random() < 0.15 ? null : Math.floor(Math.random() * 101);

        return {
            studentId: 100 + idx,
            studentName: name,
            points,
            passed: points === null ? null : points >= 50,
            dateRecorded: points === null ? null : new Date(Date.now() - Math.random() * 1000000000).toISOString(),
            url: null
        };
    });

    return {
        examId,
        examName: `Exam ${examId} – Sample Course`,
        grades
    };
}

export async function saveProfessorGradesMock(
    examId: number,
    grades: { studentId: number; points: number | null }[]
): Promise<void> {
    await delay(800);

    console.log("Mock save for exam:", examId);
    console.table(grades);

    if (Math.random() < 0.1) {
        throw new Error("Mock API error: Failed to save grades");
    }
}
