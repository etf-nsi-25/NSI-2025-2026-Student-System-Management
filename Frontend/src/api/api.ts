import type { Course } from "../component/faculty/courses/types/Course"
import type { CourseDTO } from "../dto/CourseDTO"
import type { TwoFAConfirmResponse, TwoFASetupResponse } from "../models/2fa/TwoFA.types"
import type { RestClient } from "./rest"

export class API {
    #restClient: RestClient

    constructor(restClient: RestClient) {
        this.#restClient = restClient
    }

    get<TResponse>(url: string): Promise<TResponse> {
        return this.#restClient.get<TResponse>(url)
    }

    post<TResponse>(url: string, body?: unknown): Promise<TResponse> {
        return this.#restClient.post<TResponse>(url, body)
    }

    put<TResponse>(url: string, body?: unknown): Promise<TResponse> {
        return this.#restClient.put<TResponse>(url, body)
    }

    delete<TResponse>(url: string): Promise<TResponse> {
        return this.#restClient.delete<TResponse>(url)
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    async getHelloUniversity(): Promise<any> {
        return this.#restClient.get("/api/University")
    }

    async enableTwoFactor(): Promise<TwoFASetupResponse> {
        return this.post<TwoFASetupResponse>("/api/auth/enable-2fa")
    }

    async verifyTwoFactorSetup(code: string): Promise<TwoFAConfirmResponse> {
        return this.post<TwoFAConfirmResponse>("/api/auth/verify-2fa-setup", { code })
    }

    async verifyTwoFactorLogin(code: string): Promise<TwoFAConfirmResponse> {
        return this.post<TwoFAConfirmResponse>("/api/auth/verify-2fa", { code })
    }

    async getAllCourses(): Promise<Course[]> {
        return this.get<Course[]>("/api/faculty/courses")
    }

    async getCourse(id: string): Promise<Course> {
        return this.get<Course>(`/api/faculty/courses/${id}`)
    }

    async createCourse(dto: CourseDTO): Promise<Course> {
        return this.post<Course>("/api/faculty/courses", dto)
    }

    async updateCourse(id: string, dto: CourseDTO): Promise<Course> {
        return this.put<Course>(`/api/faculty/courses/${id}`, dto)
    }

    async deleteCourse(id: string): Promise<void> {
        return this.delete<void>(`/api/faculty/courses/${id}`)
    }
}
