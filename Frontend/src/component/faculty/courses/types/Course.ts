export type Course = {
  id: string;
  name: string;
  code: string;
  type: string;
  programId: string;
  ects: number;
  professor: string;
  status?: 'enrolled' | 'pending' | 'none';
};
