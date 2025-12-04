import type { RestClient } from './rest.ts';

export class API {
    #restClient: RestClient

    constructor(restClient: RestClient) {
        this.#restClient = restClient
    }

    async getHelloUniversity(): Promise<any> {
        // DO NOT USE ANY, this is only for demonstration
        return this.#restClient.get('/api/Faculty')
    }

    get<T>(url: string) {
        return this.#restClient.get(url);
    }

    post<T>(url: string, body?: any) {
        return this.#restClient.post(url, body);
    }

    put<T>(url: string, body?: any) {
        return this.#restClient.put(url, body);
    }

    delete<T>(url: string) {
        return this.#restClient.delete(url);
    }

    // continue with other endpoints
}
