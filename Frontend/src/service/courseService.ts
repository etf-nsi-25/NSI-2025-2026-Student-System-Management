import { api } from "./rest";
import type { Course } from "../component/faculty/courses/types/Course";
import type { CourseDTO } from "./CourseDTO";

export const courseService = {
  getAll: () => api.get<Course[]>("/api/faculty/courses"),

  get: (id: string) => api.get<Course>(`/api/faculty/courses/${id}`),

  create: (dto: CourseDTO) => api.post<Course>("/api/faculty/courses", dto),

  update: (id: string, dto: CourseDTO) =>
    api.put<Course>(`/api/faculty/courses/${id}`, dto),

  delete: (id: string) => api.delete<void>(`/api/faculty/courses/${id}`),
};
