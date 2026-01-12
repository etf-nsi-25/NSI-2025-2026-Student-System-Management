import type { Course } from '../component/faculty/courses/types/Course';
import type { CourseDTO } from '../dto/CourseDTO';
import type {
    AvailableStudentExamDto,
    ExamRegistrationRequestDto,
    ExamRegistrationResponseDto,
    RegisteredStudentExamDto,
} from '../dto/StudentExamsDTO';
import type { CreateExamRequestDTO, ExamResponseDTO, UpdateExamRequestDTO } from '../dto/ExamDTO';

import type { TwoFAConfirmResponse, TwoFASetupResponse } from '../models/2fa/TwoFA.types';
import type { StudentRequestDto } from '../page/requests/RequestTypes';
import type { RestClient } from './rest';
import type { CreateFacultyRequestDTO, FacultyResponseDTO, UpdateFacultyRequestDTO } from '../dto/FacultyDTO';

export class API {
    #restClient: RestClient;

    constructor(restClient: RestClient) {
        this.#restClient = restClient;
    }

    get<T>(url: string) {
        return this.#restClient.get<T>(url);
    }

    post<T>(url: string, body?: unknown) {
        return this.#restClient.post<T>(url, body);
    }

    put<T>(url: string, body?: unknown) {
        return this.#restClient.put<T>(url, body);
    }

    delete<T>(url: string) {
        return this.#restClient.delete<T>(url);
    }

    async getHelloUniversity(): Promise<any> {
        // DO NOT USE ANY, this is only for demonstration
        return this.#restClient.get('/api/University');
    }

    async enableTwoFactor(): Promise<TwoFASetupResponse> {
        return this.#restClient.post('/api/auth/enable-2fa');
    }

    async verifyTwoFactorSetup(code: string): Promise<TwoFAConfirmResponse> {
        return this.#restClient.post('/api/auth/verify-2fa-setup', { code });
    }

    async verifyTwoFactorLogin(code: string): Promise<TwoFAConfirmResponse> {
        return this.#restClient.post('/api/auth/verify-2fa', { code });
    }

    // Course management methods
    async getAllCourses(): Promise<Course[]> {
        return this.get<Course[]>("/api/faculty/courses");
    }

    async getCourse(id: string): Promise<Course> {
        return this.get<Course>(`/api/faculty/courses/${id}`);
    }

    async createCourse(dto: CourseDTO): Promise<Course> {
        return this.post<Course>("/api/faculty/courses", dto);
    }

    async updateCourse(id: string, dto: CourseDTO): Promise<Course> {
        return this.put<Course>(`/api/faculty/courses/${id}`, dto);
    }

    async deleteCourse(id: string): Promise<void> {
        return this.delete<void>(`/api/faculty/courses/${id}`);
    }

    // Student exam registration
    async getAvailableStudentExams(): Promise<AvailableStudentExamDto[]> {
        return this.get<AvailableStudentExamDto[]>("/api/faculty/student-exams/available");
    }

    async getRegisteredStudentExams(): Promise<RegisteredStudentExamDto[]> {
        return this.get<RegisteredStudentExamDto[]>("/api/faculty/student-exams/registrations");
    }

    async registerForStudentExam(examId: number): Promise<ExamRegistrationResponseDto> {
        const body: ExamRegistrationRequestDto = { examId };
        return this.post<ExamRegistrationResponseDto>("/api/faculty/student-exams/registrations", body);
    }

    //request management 
    async getAllRequests(): Promise<StudentRequestDto[]> {
        return this.get<StudentRequestDto[]>(`/api/Support/requests`);
    }

    async updateStatus(id: string | number, status: string): Promise<{ message: string }> {
        const dto = { status };
        return this.put<{ message: string }>(`/api/Support/requests/${id}/status`, dto);
    }

    // Exam management methods
    async getExams(): Promise<ExamResponseDTO[]> {
        return this.get<ExamResponseDTO[]>('/api/exams');
    }

    async getExam(id: number | string): Promise<ExamResponseDTO> {
        return this.get<ExamResponseDTO>(`/api/exams/${id}`);
    }

    async createExam(dto: CreateExamRequestDTO): Promise<ExamResponseDTO> {
        return this.post<ExamResponseDTO>('/api/exams', dto);
    }

    async updateExam(id: number | string, dto: UpdateExamRequestDTO): Promise<ExamResponseDTO> {
        return this.put<ExamResponseDTO>(`/api/exams/${id}`, dto);
    }

    async deleteExam(id: number | string): Promise<void> {
        await this.delete<null>(`/api/exams/${id}`);
    }

    // Profile methods
    async getCurrentUser(): Promise<any> {
        return this.get<any>('/api/users/me');
    }

    async changePassword(body: any): Promise<any> {
        return this.post<any>('/api/users/me/change-password', body);
    }

    async getFaculties(): Promise<FacultyResponseDTO[]> {
        return this.get<FacultyResponseDTO[]>('/api/university/faculties');
    }

    async createFaculty(dto: CreateFacultyRequestDTO): Promise<FacultyResponseDTO> {
        return this.post<FacultyResponseDTO>('/api/university/faculties', dto);
    }

    async updateFaculty(id: number, dto: UpdateFacultyRequestDTO): Promise<FacultyResponseDTO> {
        return this.put<FacultyResponseDTO>(`/api/university/faculties/${id}`, dto);
    }

   async deleteFaculty(id: number): Promise<void> {
      return this.delete<void>(`/api/university/faculties/${id}`);
   }
}
