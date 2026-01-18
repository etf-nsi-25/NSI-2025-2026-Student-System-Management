// Student Analytics DTOs
export interface StudentSummaryDTO {
    gpa: number;
    passedSubjects: number;
    totalSubjects: number;
}

export interface WeeklyScheduleDTO {
    startHour: number;
    endHour: number;
    blocks: CourseBlockDTO[];
}

export interface CourseBlockDTO {
    id: string;
    subject: string; // Course code
    day: string; // "Mon", "Tue", "Wed", "Thu", "Fri"
    startMinutes: number; // minutes since 00:00
    endMinutes: number; // minutes since 00:00
    type?: string; // "Lecture" or "Tutorial" - from Attendance Note field
}

export interface MonthlyCalendarDTO {
    currentMonth: string; // ISO date string
    highlightedDays: HighlightedDayDTO[];
}

export interface HighlightedDayDTO {
    day: number;
    eventType: 'Exam' | 'Assignment' | 'Midterm' | 'Quiz' | 'PublicHoliday';
    eventName?: string; // Optional: Name of the event (e.g., "Final Exam", "Homework 1")
    courseCode?: string; // Optional: Course code (e.g., "RSRV", "MPVI")
}

export interface StudentAttendanceStatsDTO {
    contextLabel: string;
    items: AttendanceItemDTO[];
}

export interface AttendanceItemDTO {
    label: string; // "Lectures" or "Tutorials"
    percent: number;
    presentCount: number; // Number of present/late attendances
    totalCount: number; // Total number of attendances for this type
}

export interface SubjectProgressDTO {
    contextLabel: string;
    items: SubjectProgressItemDTO[];
}

export interface SubjectProgressItemDTO {
    code: string;
    percent: number;
    status: string; // "In Progress", "Passed", "Failed", "Enrolled"
}
