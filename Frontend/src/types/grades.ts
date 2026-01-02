export type GradeRow = {
    studentId: number;
    name: string;
    points: number | null;
    grade: number | null; 
    passed: boolean;
    maxPoints: number;
    isDirty: boolean;
    isValid: boolean;
};
