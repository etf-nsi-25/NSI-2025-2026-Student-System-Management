import type { API } from "../../api/api.ts";
import type { Course } from "../../component/faculty/courses/types/Course";

/* -------------------- COURSES -------------------- */

export async function getCourses(
    api: API,
    page = 1,
    limit = 6,
    searchQuery = "",
    filterStatus: "all" | "mandatory" | "elective" | string = "all",
) {
    try {
        let allCourses: Course[] = await api.getAllCourses();

        if (searchQuery) {
            const query = searchQuery.toLowerCase();
            allCourses = allCourses.filter(
                (course) =>
                    course.name.toLowerCase().includes(query) ||
                    course.code.toLowerCase().includes(query),
            );
        }

        if (filterStatus !== "all") {
            allCourses = allCourses.filter(
                (course) =>
                    course.type.toLowerCase() === filterStatus.toLowerCase(),
            );
        }

        const startIndex = (page - 1) * limit;
        const endIndex = startIndex + limit;

        const paginatedCourses = allCourses.slice(startIndex, endIndex);
        const hasMore = endIndex < allCourses.length;

        return {
            courses: paginatedCourses,
            hasMore,
            total: allCourses.length,
            page,
            limit,
        };
    } catch {
        return {
            courses: [],
            hasMore: false,
            total: 0,
            page,
            limit,
        };
    }
}

/* -------------------- TEACHER -------------------- */

export type TeacherDto = {
    id: number;
    fullName: string;
};

export async function getTeacherForCourse(
    api: API,
    courseId: string,
): Promise<TeacherDto> {
    return api.get<TeacherDto>(`/api/faculty/courses/${courseId}/teacher`);
}

/* -------------------- ENROLLMENTS -------------------- */

export type StudentEnrollmentItemDto = {
    enrollmentId: string;
    courseId: string;
    courseName: string;
    status: string;
    grade: number | null;
};

export async function getMyEnrollments(
    api: API,
): Promise<StudentEnrollmentItemDto[]> {
    return api.get<StudentEnrollmentItemDto[]>("/api/faculty/enrollments");
}

export type EnrollResponseDto = {
    enrollmentId: string;
    courseId: string;
    courseName: string;
    status: string;
    grade: number | null;
};

export async function enrollInCourse(api: API, courseId: string): Promise<EnrollResponseDto> {
    return api.post<EnrollResponseDto>("/api/faculty/enrollments", { courseId });
}

export async function unenrollFromCourse(
    api: API,
    enrollmentId: string,
): Promise<void> {
    await api.delete(`/api/faculty/enrollments/${enrollmentId}`);
}
