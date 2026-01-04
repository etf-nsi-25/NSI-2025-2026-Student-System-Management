export type AvailableStudentExamDto = {
  examId: number;
  courseId: string;
  courseName: string;
  courseCode: string;
  examName: string;
  examDate: string;
  registrationDeadline: string | null;
};

export type RegisteredStudentExamDto = {
  registrationId: number;
  examId: number;
  courseId: string;
  courseName: string;
  courseCode: string;
  examName: string;
  examDate: string | null;
  registrationDate: string;
  status: string;
};

export type ExamRegistrationRequestDto = {
  examId: number;
};

export type ExamRegistrationResponseDto = {
  registrationId: number;
  message: string;
};
