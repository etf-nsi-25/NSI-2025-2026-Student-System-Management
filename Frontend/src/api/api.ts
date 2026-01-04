import type { Course } from '../component/faculty/courses/types/Course';
import type { CourseDTO } from '../dto/CourseDTO';

import type { TwoFAConfirmResponse, TwoFASetupResponse } from '../models/2fa/TwoFA.types';
import type { RestClient } from './rest';

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
        // nema body-a, userId je za sada hardcodan u backendu ("demo")
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
}
