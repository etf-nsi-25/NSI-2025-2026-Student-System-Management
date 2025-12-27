import { API } from "../../api/api";
import type { CreateIssueRequest, FAQ } from "./types";

export async function getFaqs(api: API): Promise<FAQ[]> {
    return api.get<FAQ[]>("/api/faqs");
}

export async function createIssue(api: API, request: CreateIssueRequest): Promise<void> {
    return api.post<void>("/api/issue", request);
}
