export interface Faculty {
    id: string;
    name: string;
}

export interface Program {
    id: string;
    name: string;
    facultyId: string;
}

export interface Course {
    id: string;
    name: string;
    code: string;
    programId: string;
}

export type AttendanceStatus = 'Present' | 'Absent' | 'Late';

export interface Student {
    id: number;
    firstName: string;
    lastName: string;
    indexNumber: string;
    avatarUrl?: string;
}

export interface AttendanceRecord {
    id: number;
    studentId: number;
    courseId: string;
    lectureDate: string; // ISO Date string
    status: AttendanceStatus | null;
    note?: string;
    student: Student;
}

export interface AttendanceFilter {
    facultyId?: string;
    programId?: string;
    courseId?: string;
    date?: string;
}

export interface AttendanceStats {
    present: number;
    absent: number;
    late: number;
}
