import type { ExamDTO } from "./Exam.types";

export interface ExamRegistrationDTO {
    id: number;
    examId: number;
    studentId: number;
    registrationDate: string;
    status: string;
    exam: ExamDTO;
}
