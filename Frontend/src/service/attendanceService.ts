import type { Program, AttendanceRecord, AttendanceStats, Student } from "../models/attendance/Attendance.types";
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

type EnrolledStudentDto = {
    id: number;
    firstName: string;
    lastName: string;
    indexNumber: string;
    avatarUrl?: string;
};

const MOCK_PROGRAMS: Program[] = [
    { id: 'p1', name: 'Computer Science', facultyId: 'f1' },
    { id: 'p2', name: 'Electrical Engineering', facultyId: 'f1' },
    { id: 'p3', name: 'Physics', facultyId: 'f2' },
    { id: 'p4', name: 'History', facultyId: 'f3' },
];

// Helper to simulate delay for mocked pieces (programs only)
const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

export const getPrograms = async (_facultyId: string): Promise<Program[]> => {
    await delay(300);
    return MOCK_PROGRAMS;
};

export const getAttendance = async (api: API, courseId: string, date: string): Promise<AttendanceRecord[]> => {
    const enrolled = await api.get<EnrolledStudentDto[]>(
        `/api/faculty/attendance/enrolled-students?courseId=${encodeURIComponent(courseId)}&date=${encodeURIComponent(date)}`
    );

    const lectureDate = date;

    return enrolled.map((s): AttendanceRecord => ({
        id: s.id,
        studentId: s.id,
        courseId,
        lectureDate,
        status: null,
        note: '',
        student: {
            id: s.id,
            firstName: s.firstName,
            lastName: s.lastName,
            indexNumber: s.indexNumber,
            avatarUrl: s.avatarUrl,
        } as Student,
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
    const url = `/api/faculty/attendance/export?courseId=${encodeURIComponent(courseId)}&date=${encodeURIComponent(date)}`;

    if (typeof window !== 'undefined') {
        window.open(url, '_blank');
    }

    return true;
};

export const getAttendanceStats = async (api: API, courseId: string, month: string): Promise<AttendanceStats> => {
    const stats = await api.get<AttendanceStats>(
        `/api/faculty/attendance/stats?courseId=${encodeURIComponent(courseId)}&month=${encodeURIComponent(month)}`
    );
    return stats;
};
