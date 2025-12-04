import { api } from './rest';
import type { Course } from '../component/faculty/courses/types/Course';

export const courseService = {
  getAll: () => api.get<Course[]>('/api/Course'),

  get: (id: string) => api.get<Course>(`/api/Course/${id}`),

  create: (data: Course) => api.post<Course>('/api/Course', data),

  update: (id: string, data: Course) =>
    api.put<Course>(`/api/Course/${id}`, data),

  delete: (id: string) => api.delete<void>(`/api/Course/${id}`)
};
