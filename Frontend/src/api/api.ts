import type { RestClient } from './rest';

import type {
    TwoFASetupResponse,
    TwoFAConfirmResponse,
} from '../models/2fa/TwoFA.types';

export class API {
    #restClient: RestClient;

    constructor(restClient: RestClient) {
        this.#restClient = restClient;
    }

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
}
