export interface ExamDTO {
    id: number | string;
    name: string;
    status: 'COMPLETED' | 'UPCOMING' | 'FAILED';
    date: string;
    registerable?: boolean;
}

export interface AssignmentDTO {
    id: number | string;
    name: string;
    desc: string;
    dueDate?: string;
    status: 'todo' | 'marked';
    points?: string;
}

export interface CourseOverviewDTO {
    name: string;
    professor: string;
    ects: number;
    progress: number;
    attendance: {
        present: number;
        total: number;
        percentage: number;
    };
    exams: ExamDTO[];
    assignments: AssignmentDTO[];
}