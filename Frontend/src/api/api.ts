import type { Course } from "../component/faculty/courses/types/Course"
import type { CourseDTO } from "../dto/CourseDTO"
import type { TwoFAConfirmResponse, TwoFASetupResponse } from "../models/2fa/TwoFA.types"
import type { RestClient } from "./rest"

export class API {
    #restClient: RestClient

    constructor(restClient: RestClient) {
        this.#restClient = restClient
    }

    get<T>(url: string): Promise<T> {
        return this.#restClient.get<T>(url)
    }

    post<TResponse, TBody = unknown>(url: string, body?: TBody): Promise<TResponse> {
        return this.#restClient.post<TResponse>(url, body)
    }

    put<TResponse, TBody = unknown>(url: string, body?: TBody): Promise<TResponse> {
        return this.#restClient.put<TResponse>(url, body)
    }

    delete<T>(url: string): Promise<T> {
        return this.#restClient.delete<T>(url)
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    async getHelloUniversity(): Promise<any> {
        return this.#restClient.get("/api/University")
    }


    async enableTwoFactor(): Promise<TwoFASetupResponse> {
        return this.#restClient.post("/api/auth/enable-2fa")
    }

    async verifyTwoFactorSetup(code: string): Promise<TwoFAConfirmResponse> {
        return this.#restClient.post("/api/auth/verify-2fa-setup", { code })
    }

    async verifyTwoFactorLogin(code: string): Promise<TwoFAConfirmResponse> {
        return this.#restClient.post("/api/auth/verify-2fa", { code })
    }

    // Course management methods
    async getAllCourses(): Promise<Course[]> {
        return this.get<Course[]>("/api/faculty/courses")
    }

    async getCourse(id: string): Promise<Course> {
        return this.get<Course>(`/api/faculty/courses/${id}`)
    }

    async createCourse(dto: CourseDTO): Promise<Course> {
        return this.post<Course, CourseDTO>("/api/faculty/courses", dto)
    }

    async updateCourse(id: string, dto: CourseDTO): Promise<Course> {
        return this.put<Course, CourseDTO>(`/api/faculty/courses/${id}`, dto)
    }

    async deleteCourse(id: string): Promise<void> {
        return this.delete<void>(`/api/faculty/courses/${id}`)
    }
}
