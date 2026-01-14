export type FacultyResponseDTO = {
  id: number;
  name: string;
  address: string;
  code: string;
  createdAt: string;
  updatedAt?: string;
};

export type CreateFacultyRequestDTO = {
  name: string;
  address: string;
  code: string;
};

export type UpdateFacultyRequestDTO = CreateFacultyRequestDTO;
