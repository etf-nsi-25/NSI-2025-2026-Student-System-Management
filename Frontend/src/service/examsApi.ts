export interface Exam {
  id: string;
  courseName: string;
  dateTime: string;
  location: string;
}

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...init,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...(init?.headers ?? {}),
    },
  });

  const text = await (response as any).text();
  if (!response.ok) {
    throw new Error(text || `Request failed (${response.status})`);
  }

  return (text ? JSON.parse(text) : null) as T;
}

export const fetchExams = async (): Promise<Exam[]> => {
  return request<Exam[]>('/api/exams');
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
  return request<Exam>(`/api/exams/${id}`);
};

export const createExam = async (
  payload: CreateExamPayload,
): Promise<void> => {
  await request<unknown>('/api/exams', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
};

export const updateExam = async (
  id: string,
  payload: UpdateExamPayload,
): Promise<void> => {
  await request<unknown>(`/api/exams/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
};

export const deleteExam = async (id: string): Promise<void> => {
  await request<unknown>(`/api/exams/${id}`, {
    method: 'DELETE',
  });
};
