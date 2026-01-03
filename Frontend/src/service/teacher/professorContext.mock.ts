export async function getProfessorContextMock() {
    await new Promise(r => setTimeout(r, 500));

    return {
        semesters: ["Winter 2025", "Summer 2025", "Winter 2026"],
        courses: [
            { id: 1, name: "Algorithms and Data Structures" },
            { id: 2, name: "Software Engineering" },
            { id: 3, name: "Database Systems" }
        ],
        exams: [
            { id: 1, name: "Midterm Exam" },
            { id: 2, name: "Final Exam" },
            { id: 3, name: "Project Presentation" }
        ]
    };
}
