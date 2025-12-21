//with dummy data
import type { Enrollment, Faculty } from "../../models/enrollment/Enrollment.types";

const FACULTIES_MOCK: Faculty[] = [
  { id: "etf", name: "ETF" },
  { id: "pmf", name: "PMF" },
  { id: "med", name: "Medical Faculty" },
];

let ENROLLMENTS_MOCK: Enrollment[] = [
  {
    id: "1",
    facultyId: "etf",
    facultyName: "ETF",
    date: "2025-11-15",
    academicYear: "2025/2026",
    status: "Pending",
  },
  {
    id: "2",
    facultyId: "etf",
    facultyName: "ETF",
    date: "2025-10-13",
    academicYear: "2024/2025",
    status: "Done",
  },
  {
    id: "3",
    facultyId: "etf",
    facultyName: "ETF",
    date: "2025-09-04",
    academicYear: "2023/2024",
    status: "Done",
  },
  {
    id: "4",
    facultyId: "eyf",
    facultyName: "EYF",
    date: "2025-09-01",
    academicYear: "2022/2023",
    status: "Done",
  },
];

const delay = (ms: number) => new Promise((res) => setTimeout(res, ms));

export const studentEnrollmentService = {
  async getFaculties(): Promise<Faculty[]> {
    await delay(300);
    return FACULTIES_MOCK;
  },

  async getEnrollments(): Promise<Enrollment[]> {
    await delay(300);
    return ENROLLMENTS_MOCK;
  },

async createEnrollment(academicYear: string): Promise<void> {
  await delay(400);

  const existingEnrollment = ENROLLMENTS_MOCK[0];

  if (!existingEnrollment) {
    throw new Error("Cannot determine student faculty");
  }

  const facultyId = existingEnrollment.facultyId;

  const faculty = FACULTIES_MOCK.find((f) => f.id === facultyId);

  if (!faculty) {
    throw new Error("Faculty not found");
  }

  const newEnrollment: Enrollment = {
    id: crypto.randomUUID(),
    facultyId: faculty.id,
    facultyName: faculty.name,
    date: new Date().toISOString(),
    academicYear,
    status: "Pending",
  };

  ENROLLMENTS_MOCK = [newEnrollment, ...ENROLLMENTS_MOCK];
},
};
