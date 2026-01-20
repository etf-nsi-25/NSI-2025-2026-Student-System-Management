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
    subject: string;
    day: string;
    startMinutes: number;
    endMinutes: number;
    type?: string;
}

export interface MonthlyCalendarDTO {
    currentMonth: string;
    highlightedDays: HighlightedDayDTO[];
}

export interface HighlightedDayDTO {
    day: number;
    eventType: 'Exam' | 'Assignment' | 'Midterm' | 'Quiz' | 'PublicHoliday';
    eventName?: string;
    courseCode?: string;
}

export interface StudentAttendanceStatsDTO {
    contextLabel: string;
    items: AttendanceItemDTO[];
}

export interface AttendanceItemDTO {
    label: string;
    percent: number;
    presentCount: number;
    totalCount: number;
}

export interface SubjectProgressDTO {
    contextLabel: string;
    items: SubjectProgressItemDTO[];
}

export interface SubjectProgressItemDTO {
    code: string;
    percent: number;
    status: string;
}
