export interface ExamDTO {
    id: number;
    courseId: string;
    courseName: string;
    courseCode: string;
    examDate: string | null;
    regDeadline: string | null;
    location: string;            
}
