import type { RestClient } from './rest';

export class API {
    #restClient: RestClient;

    constructor(restClient: RestClient) {
        this.#restClient = restClient;
    }

    // ← Ovo su univerzalne metode tako da courseService radi kako treba
    get<T>(url: string) {
        return this.#restClient.get<T>(url);
    }

    post<T>(url: string, body?: any) {
        return this.#restClient.post<T>(url, body);
    }

    put<T>(url: string, body?: any) {
        return this.#restClient.put<T>(url, body);
    }

    delete<T>(url: string) {
        return this.#restClient.delete<T>(url);
    }
}
