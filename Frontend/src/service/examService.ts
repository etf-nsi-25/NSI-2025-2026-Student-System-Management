import type { ExamDTO } from "../models/exams/Exam.types";
import type { ExamRegistrationDTO } from "../models/exams/ExamRegistration.types";

export const examService = {
  async getEligibleExams(): Promise<ExamDTO[]> {
    return [];
  },

  async getRegisteredExams(): Promise<ExamRegistrationDTO[]> {
    return [];
  },

  async registerForExam(examId: number): Promise<void> {
    if (examId === 999) {
      const error: any = new Error("Already registered");
      error.status = 400;
      throw error;
    }
  },

  async unregisterExam(examId: number): Promise<void> {
    void examId;
    return;
  }
};
