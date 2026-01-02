export type GradeResponse = {
    studentId: number;
    studentName: string;
    points: number | null;
    passed: boolean | null;
    dateRecorded?: string;
    url?: string;
}

export type StudentGradeListResponse = {
    examId: number;
    examName: string;
    grades: GradeResponse[];
}
