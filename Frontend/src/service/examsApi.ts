export interface Exam {
  id: string;
  courseName: string;
  dateTime: string;
  location: string;
}

import { api } from '../api/rest';

export const fetchExams = async (): Promise<Exam[]> => {
  return api.get<Exam[]>('/api/exams');
};

/* =========================
   CREATE EXAM (TASK 444)
   ========================= */

export interface CreateExamPayload {
  courseName: string;
  dateTime: string;
  location: string;
}

export type UpdateExamPayload = CreateExamPayload;

export const getExam = async (id: string): Promise<Exam> => {
  return api.get<Exam>(`/api/exams/${id}`);
};

export const createExam = async (
  payload: CreateExamPayload,
): Promise<void> => {
  await api.post<unknown>('/api/exams', payload);
};

export const updateExam = async (
  id: string,
  payload: UpdateExamPayload,
): Promise<void> => {
  await api.put<unknown>(`/api/exams/${id}`, payload);
};

export const deleteExam = async (id: string): Promise<void> => {
  await api.delete<unknown>(`/api/exams/${id}`);
};
