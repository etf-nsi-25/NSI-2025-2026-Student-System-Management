import {
    getProfessorGradesMock,
    saveProfessorGradesMock
} from "./professorGrades.mock";

export function getProfessorGrades(examId: number) {
    return getProfessorGradesMock(examId);
}

export function saveProfessorGrades(
    examId: number,
    grades: { studentId: number; points: number | null }[]
) {
    return saveProfessorGradesMock(examId, grades);
}
