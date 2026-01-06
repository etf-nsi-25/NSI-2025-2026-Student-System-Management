export type GradeResponse = {
    studentId: number;
    studentName: string;
    points: number | null;
    passed: boolean | null;
    dateRecorded: string | null;
    url?: string | null;
}

export type StudentGradeListResponse = {
    examId: number;
    examName: string;
    grades: GradeResponse[];
}
