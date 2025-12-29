import type { AuthInfo } from '../init/auth.tsx';
import { API } from './api.ts';

type Method = 'GET' | 'POST' | 'PUT' | 'DELETE';

export type ErrorResponse = {
    message: Promise<string>;
    status: number;
};

type APIResponse<T> = {
    successResponse?: Promise<T>;
    errorResponse?: ErrorResponse;
    ok: boolean;
};

export class RestClient {
    #authInfo: AuthInfo;
    #authFailCallback: () => Promise<AuthInfo | undefined>;

    /**
     * Instantiates a new {@link API} service.
     *
     * @param authInfo auth info necessary to construct auth headers
     * @param authFailCallback in case of 401 or 403, allows us to attempt silent refresh of token
     */
    constructor(authInfo: AuthInfo, authFailCallback: () => Promise<AuthInfo | undefined>) {
        this.#authInfo = authInfo;
        this.#authFailCallback = authFailCallback;
    }

    async get<T>(url: string): Promise<T> {
        return this.#submitRequestWithFallback<T>(url, 'GET');
    }

    async post<T>(url: string, body?: any): Promise<T> {
        return this.#submitRequestWithFallback<T>(url, 'POST', body);
    }

    async put<T>(url: string, body?: any): Promise<T> {
        return this.#submitRequestWithFallback<T>(url, 'PUT', body);
    }

    async delete<T>(url: string): Promise<T> {
        return this.#submitRequestWithFallback<T>(url, 'DELETE');
    }


    async #submitRequestWithFallback<T>(
        url: string,
        method: Method,
        body?: unknown
    ): Promise<T> {
        return this.#submitRequest<T>(url, method, body).then(
            async (apiResponse) => {
                if (apiResponse.ok) {
                    return apiResponse.successResponse!;
                }

                const errorResponse = await apiResponse.errorResponse!;
                if (errorResponse.status === 401) {
                    // Attempt to fetch one more time after token refresh
                    const newToken = await this.#authFailCallback();
                    if (!newToken) {
                        // TODO: this could have problems in high concurrency. Implement aborting in-flight requests if needed.
                        throw {
                            message: errorResponse.message,
                            status: errorResponse.status
                        }
                    }

                    this.#authInfo = newToken;

                    const result = await this.#submitRequest<T>(url, method, body);
                    if (!result.ok) {
                        throw {
                            message: await apiResponse.errorResponse?.message,
                            status: apiResponse.errorResponse?.status
                        };
                    } else {
                        return result.successResponse!;
                    }
                } else {
                    throw {
                        message: await apiResponse.errorResponse?.message,
                        status: apiResponse.errorResponse?.status
                    };
                }
            }
        );
    }

    async #submitRequest<T>(url: string, method: Method, body?: unknown): Promise<APIResponse<T>> {
        return fetch(
            url,
            {
                method: method,
                body: JSON.stringify(body),
                headers: {
                    'Authorization': `Bearer ${ this.#authInfo.accessToken }`,
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: 'include'
            }
        ).then(async response => {
            if (!response.ok) {
                return {
                    ok: false,
                    errorResponse: { message: response.text(), status: response.status }
                }
            } else {
                return {
                    ok: true,
                    successResponse: response.text().then(text => (text ? JSON.parse(text) : null) as T)
                }
            }
        })
    }
}
