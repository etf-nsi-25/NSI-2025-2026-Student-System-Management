import type { Faculty, Program, Course, AttendanceRecord, AttendanceStats } from "../models/attendance/Attendance.types";
import type { API } from "../api/api";

type SaveAttendanceRequestDTO = {
    courseId: string;
    date: string;
    records: Array<{
        studentId: number;
        status: NonNullable<AttendanceRecord["status"]>;
        note?: string;
    }>;

};

// Mock Data
const MOCK_FACULTIES: Faculty[] = [
    { id: 'f1', name: 'Faculty of Engineering' },
    { id: 'f2', name: 'Faculty of Science' },
    { id: 'f3', name: 'Faculty of Arts' },
];

const MOCK_PROGRAMS: Program[] = [
    { id: 'p1', name: 'Computer Science', facultyId: 'f1' },
    { id: 'p2', name: 'Electrical Engineering', facultyId: 'f1' },
    { id: 'p3', name: 'Physics', facultyId: 'f2' },
    { id: 'p4', name: 'History', facultyId: 'f3' },
];

const MOCK_COURSES: Course[] = [
    { id: 'c1', name: 'Introduction to Programming', code: 'CS101', programId: 'p1' },
    { id: 'c2', name: 'Data Structures', code: 'CS102', programId: 'p1' },
    { id: 'c3', name: 'Circuit Analysis', code: 'EE101', programId: 'p2' },
    { id: 'c4', name: 'Quantum Mechanics', code: 'PHY201', programId: 'p3' },
    { id: 'c5', name: 'World History', code: 'HIS101', programId: 'p4' },
];

const MOCK_STUDENTS = [
    { id: 1, firstName: 'John', lastName: 'Doe', indexNumber: '12345', avatarUrl: 'https://ui-avatars.com/api/?name=John+Doe' },
    { id: 2, firstName: 'Jane', lastName: 'Smith', indexNumber: '12346', avatarUrl: 'https://ui-avatars.com/api/?name=Jane+Smith' },
    { id: 3, firstName: 'Alice', lastName: 'Johnson', indexNumber: '12347', avatarUrl: 'https://ui-avatars.com/api/?name=Alice+Johnson' },
    { id: 4, firstName: 'Bob', lastName: 'Brown', indexNumber: '12348', avatarUrl: 'https://ui-avatars.com/api/?name=Bob+Brown' },
    { id: 5, firstName: 'Charlie', lastName: 'Davis', indexNumber: '12349', avatarUrl: 'https://ui-avatars.com/api/?name=Charlie+Davis' },
];

// Helper to simulate delay
const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

export const getFaculties = async (): Promise<Faculty[]> => {
    await delay(500);
    return MOCK_FACULTIES;
};

export const getPrograms = async (facultyId: string): Promise<Program[]> => {
    await delay(300);
    return MOCK_PROGRAMS.filter(p => p.facultyId === facultyId);
};

export const getCourses = async (programId: string): Promise<Course[]> => {
    await delay(300);
    return MOCK_COURSES.filter(c => c.programId === programId);
};

export const getAttendance = async (courseId: string, date: string): Promise<AttendanceRecord[]> => {
    await delay(800);
    return MOCK_STUDENTS.map(student => ({
        id: Math.floor(Math.random() * 10000),
        studentId: student.id,
        courseId: courseId,
        lectureDate: date,
        status: Math.random() > 0.2 ? (Math.random() > 0.5 ? 'Present' : 'Absent') : null, // Random status or null
        note: '',
        student: student
    }));
};

export const saveAttendance = async (api: API, attendanceData: AttendanceRecord[]): Promise<boolean> => {
    const filtered = attendanceData.filter(r => r.status !== null);
    if (filtered.length === 0) return true;
    const courseId = filtered[0].courseId;
    const date = filtered[0].lectureDate;

    const request: SaveAttendanceRequestDTO = {
        courseId,
        date,
        records: filtered.map(r => ({
            studentId: r.studentId,
            status: r.status!,
            note: r.note ?? undefined,
        })),
    };

    await api.post<void>("/api/faculty/attendance", request);
    return true;
};

export const exportAttendance = async (courseId: string, date: string): Promise<boolean> => {
    await delay(1000);
    console.log(`Exporting attendance for course ${courseId} on ${date}`);
    return true;
};

export const getAttendanceStats = async (courseId: string, month: string): Promise<AttendanceStats> => {
    await delay(600);
    const seed = courseId.length + month.length; 
    return {
        present: 60 + (seed % 20),
        absent: 10 + (seed % 10),
        late: 5 + (seed % 5)
    };
};
