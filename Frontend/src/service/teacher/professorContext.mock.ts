export async function getProfessorContextMock() {
    await new Promise(r => setTimeout(r, 500));

    return {
        semesters: ["Winter 2025", "Summer 2025", "Winter 2026"],
        courses: [
            { id: 1, name: "Algorithms and Data Structures", semester: "Winter 2025", exams: [1,2] },
            { id: 2, name: "Software Engineering", semester: "Winter 2025", exams: [3] },
            { id: 3, name: "Database Systems", semester: "Winter 2025", exams: [4] }
        ],
        exams: [
            { id: 1, name: "Midterm", courseId: 1 },
            { id: 2, name: "Final Exam", courseId: 1 },
            { id: 3, name: "Project Presentation", courseId: 2 },
            { id: 4, name: "Midterm", courseId: 3 }
        ]
    };
}
