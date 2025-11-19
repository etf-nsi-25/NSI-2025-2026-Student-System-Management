export type Course = {
  id: string;
  name: string;
  faculty: string;
  ects: number;
  semester: number;
  major: string;
  professor?: string;
  assistant?: string;
};