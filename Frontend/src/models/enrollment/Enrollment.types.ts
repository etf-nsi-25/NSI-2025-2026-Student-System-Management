export interface Course {
  id: string;
  name: string;
  code: string;
  professor: string;
  ects: number;
  status: "required" | "elective" | "enrolled";
}
export type EnrollmentStatus = "Pending" | "Done" | "Rejected";

export interface Faculty {
  id: string;
  name: string;
}

export interface Enrollment {
  id: string;
  facultyId: string;
  facultyName: string;
  date: string;
  academicYear: string;
  status: EnrollmentStatus;
}
