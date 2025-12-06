import type { RestClient } from './rest.ts';
import type { TwoFASetupResponse, TwoFAConfirmResponse } from '../models/2fa/TwoFA.types';

export class API {
    #restClient: RestClient

    constructor(restClient: RestClient) {
        this.#restClient = restClient
    }

    async getHelloUniversity(): Promise<any> {
        return this.#restClient.get('/api/University')
    }

    // 2FA ENDPOINTS 

    async enableTwoFactor(): Promise<TwoFASetupResponse> {
        // nema body-a, userId je za sada hardcodan u backendu ("demo")
        return this.#restClient.post('/api/auth/enable-2fa');
    }

    async verifyTwoFactorSetup(code: string): Promise<TwoFAConfirmResponse> {
        return this.#restClient.post('/api/auth/enable-2fa/confirm', { code });
    }

    async verifyTwoFactorLogin(code: string): Promise<TwoFAConfirmResponse> {
        return this.#restClient.post('/api/auth/verify-2fa', { code });
    }

    // add other endpoints here
}
