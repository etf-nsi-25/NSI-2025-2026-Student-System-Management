import type { AuthInfo } from '../init/auth.tsx';

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
  #baseUrl: string;
  #authInfo: AuthInfo;
  #authFailCallback: () => void;

  /**
   * Instantiates a new {@link API} service.
   *
   * @param authInfo auth info necessary to construct auth headers
   * @param authFailCallback in case of 401 or 403, allows us to attempt silent refresh of token
   * @param baseUrl base URL for the API endpoints
   */
  constructor(authInfo: AuthInfo, authFailCallback: () => void, baseUrl: string) {
    this.#baseUrl = baseUrl;
    this.#authInfo = authInfo;
    this.#authFailCallback = authFailCallback;
  }

  async get<T>(url: string): Promise<T> {
    return this.#submitRequestWithFallback<T>(this.#baseUrl + url, 'GET');
  }

  async post<T, B>(url: string, body?: B): Promise<T> {
    return this.#submitRequestWithFallback<T>(this.#baseUrl + url, 'POST', body);
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
        if (errorResponse.status === 401 || errorResponse.status === 403) {
          // Attempt to fetch one more time after token refresh
          this.#authFailCallback();

          const result = await this.#submitRequest<T>(url, method, body);

          if (!result.ok) {
            throw {
              message: await apiResponse.errorResponse?.message,
              status: apiResponse.errorResponse?.status,
            };
          } else {
            return result.successResponse!;
          }
        } else {
          throw {
            message: await apiResponse.errorResponse?.message,
            status: apiResponse.errorResponse?.status,
          };
        }
      }
    );
  }

  async #submitRequest<T>(
    url: string,
    method: Method,
    body?: unknown
  ): Promise<APIResponse<T>> {
    return fetch(url, {
      method: method,
      body: JSON.stringify(body),
      headers: {
        Authorization: `${this.#authInfo.accessToken}`,
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      credentials: 'include', // Include cookies in requests
    }).then(async (response) => {
      if (!response.ok) {
        return {
          ok: false,
          errorResponse: { message: response.text(), status: response.status },
        };
      } else {
        return {
          ok: true,
          successResponse: response
            .text()
            .then((text) => (text ? JSON.parse(text) : null) as T),
        };
      }
    });
  }
}
