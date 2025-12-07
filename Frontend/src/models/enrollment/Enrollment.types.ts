export interface Course {
  id: string
  name: string
  code: string
  professor: string
  ects: number
  status: "required" | "elective" | "enrolled"
}