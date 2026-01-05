export interface Course {
  id: string;
  name: string;
  code: string;
  professor: string;
  ects: number;
  status: "required" | "elective" | "enrolled";
}

export interface Faculty {
  id: string;
  name: string;
}

export type EnrollmentRequestStatus = "Pending" | "Approved" | "Rejected";

export interface EnrollmentRequest {
  id: string;
  createdAt: string;      
  academicYear: string;
  semester: number;
  status: EnrollmentRequestStatus;
}

