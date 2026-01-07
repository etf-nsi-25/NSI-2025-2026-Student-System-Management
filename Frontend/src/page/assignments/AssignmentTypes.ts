export interface Assignment {
    assignmentId: number;
    title: string;
    description?: string;
    dueDate?: string;
    maxPoints?: number;
    status: string;
    submissionDate?: string;
    grade?: number;
    points?: number;
    feedback?: string;
}