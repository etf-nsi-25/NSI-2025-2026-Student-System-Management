import type { AttendanceRecord, AttendanceStats, Student } from "../models/attendance/Attendance.types";
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
    studentId: number;
    firstName: string;
    lastName: string;
    indexNumber: string;
    avatarUrl?: string;
    status: AttendanceRecord["status"];
    note?: string | null;
};

type AttendanceStatsResponseDto = {
    totalRecords: number;
    presentCount: number;
    absentCount: number;
    lateCount: number;
    presentPercentage: number;
    absentPercentage: number;
    latePercentage: number;
};

export const getAttendance = async (api: API, courseId: string, date: string): Promise<AttendanceRecord[]> => {
    const enrolled = await api.get<EnrolledStudentDto[]>(
        `/api/faculty/attendance/courses/${encodeURIComponent(courseId)}/date/${encodeURIComponent(date)}`
    );

    const lectureDate = date;

    return enrolled.map((s): AttendanceRecord => ({
        id: s.studentId,
        studentId: s.studentId,
        courseId,
        lectureDate,
        status: s.status ?? null,
        note: s.note ?? '',
        student: {
            id: s.studentId,
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

export const exportAttendance = async (courseId: string, date: string, accessToken?: string): Promise<boolean> => {
    const url = `/api/faculty/attendance/courses/${encodeURIComponent(courseId)}/date/${encodeURIComponent(date)}/export`;

    if (typeof window === 'undefined') {
        return false;
    }

    if (!accessToken) {
        window.open(url, '_blank');
        return true;
    }

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${accessToken}`,
            },
            credentials: 'include',
        });

        if (!response.ok) {
            throw new Error('Failed to export attendance');
        }

        const blob = await response.blob();
        const contentDisposition = response.headers.get('Content-Disposition');

        let filename = 'attendance-export';
        if (contentDisposition) {
            const match = /filename="?([^";]+)"?/i.exec(contentDisposition);
            if (match && match[1]) {
                filename = match[1];
            }
        }

        const downloadUrl = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = downloadUrl;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(downloadUrl);

        return true;
    } catch (error) {
        console.error('Error exporting attendance', error);
        return false;
    }
};

export const getAttendanceStats = async (api: API, courseId: string, month: string): Promise<AttendanceStats> => {
    const [yearStr, monthStr] = month.split("-");
    const year = Number(yearStr);
    const monthIndex = Number(monthStr); 

    if (!year || !monthIndex) {
        throw new Error(`Invalid month format for attendance statistics: ${month}`);
    }

    const startDate = new Date(year, monthIndex - 1, 1);
    const endDate = new Date(year, monthIndex, 0); 

    const startDateStr = startDate.toISOString().split("T")[0];
    const endDateStr = endDate.toISOString().split("T")[0];

    const stats = await api.get<AttendanceStatsResponseDto>(
        `/api/faculty/attendance/courses/${encodeURIComponent(courseId)}/statistics?startDate=${encodeURIComponent(startDateStr)}&endDate=${encodeURIComponent(endDateStr)}`
    );
    return {
        present: stats.presentCount,
        absent: stats.absentCount,
        late: stats.lateCount,
    };
};
