import type { AuthInfo } from '../init/auth.tsx';

type Method = 'GET' | 'POST' | 'PUT' | 'DELETE'

export type ErrorResponse = {
    message: Promise<string>,
    status: number
}

type APIResponse<T> = {
    successResponse?: Promise<T>,
    errorResponse?: ErrorResponse
    ok: boolean
}

export class API {
    #authInfo: AuthInfo
    #authFailCallback: () => void;

    /**
     * Instantiates a new {@link API} service.
     *
     * @param authInfo auth info necessary to construct auth headers
     * @param authFailCallback in case of 401 or 403, allows us to attempt silent refresh of token
     */
    constructor(authInfo: AuthInfo, authFailCallback: () => void) {
        this.#authInfo = authInfo
        this.#authFailCallback = authFailCallback;
    }

    // University
    async getHelloUniversity(): Promise<any> {
        // DO NOT USE ANY, this is only for demonstration
        return this.#submitRequestWithFallback('/api/University', 'GET')
    }

    // continue with other endpoints

    async #submitRequest<T>(url: string, method: Method, body?: unknown): Promise<APIResponse<T>> {
        return fetch(
            url,
            {
                method: method,
                body: JSON.stringify(body),
                headers: {
                    'Authorization': `${ this.#authInfo.accessToken }`,
                    'Accept': 'application/json'
                }
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

    async #submitRequestWithFallback<T>(url: string, method: Method, body?: unknown) {
        return this.#submitRequest<T>(url, method, body)
            .then(async apiResponse => {
                if (apiResponse.ok) {
                    return apiResponse.successResponse!
                }

                const errorResponse = await apiResponse.errorResponse!
                if (errorResponse.status === 401 || errorResponse.status === 403) {
                    // Attempt to fetch one more time after token refresh
                    this.#authFailCallback();

                    const result = await this.#submitRequest<T>(url, method, body);

                    if (!result.ok) {
                        await this.#handleErrorResponse(result)
                    } else {
                        return result.successResponse
                    }
                } else {
                    await this.#handleErrorResponse(apiResponse)
                }
            }).catch(error => {
                throw {
                    message: error.message,
                    status: error.status
                }
            })

    }

    async #handleErrorResponse<T>(response: APIResponse<T>): Promise<void> {
        throw {
            message: await response.errorResponse?.message,
            status: response.errorResponse?.status
        }
    }
}
