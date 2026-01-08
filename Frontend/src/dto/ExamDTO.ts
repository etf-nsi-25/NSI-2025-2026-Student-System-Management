export type ExamType = 'Written' | 'Oral' | 'Practical' | 'Online';

export type ExamResponseDTO = {
  id: number;
  courseId: string;
  courseName?: string;
  name?: string;
  location?: string;
  examType: ExamType;
  examDate?: string;
  regDeadline?: string;
  createdAt: string;
  updatedAt?: string;
};

export type CreateExamRequestDTO = {
  courseId: string;
  name: string;
  location: string;
  examType: ExamType;
  examDate: string;
  regDeadline: string;
};

export type UpdateExamRequestDTO = CreateExamRequestDTO;
