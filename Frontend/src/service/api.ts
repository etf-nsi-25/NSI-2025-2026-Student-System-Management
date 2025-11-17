import type { RestClient } from "./rest.ts";

export class API {
  #restClient: RestClient;

  constructor(restClient: RestClient) {
    this.#restClient = restClient;
  }

  async getHelloUniversity(): Promise<any> {
    // DO NOT USE ANY, this is only for demonstration
    return this.#restClient.get("/api/University");
  }

  async login(email: string, password: string): Promise<any> {
    return this.#restClient.post("https://localhost:5001/api/auth/login", {
      email,
      password,
    });
  }

  // continue with other endpoints
}
