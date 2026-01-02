// service/enrollment/studentEnrollmentService.ts
import type { API } from "../../api/api";
import type { EnrollmentRequest } from "../../models/enrollment/Enrollment.types";

export const studentEnrollmentService = {
  async getEnrollments(api: API, userId: string): Promise<EnrollmentRequest[]> {
    return api.get<EnrollmentRequest[]>(
      `/api/support/enrollment-requests/my?userId=${userId}`
    );
  },

  async createEnrollment(
    api: API,
    payload: {
      userId: string;
      facultyId: string;
      academicYear: string;
      semester: number;
    }
  ): Promise<void> {
    return api.post<void>("/api/support/enrollment-requests", payload);
  },
};
