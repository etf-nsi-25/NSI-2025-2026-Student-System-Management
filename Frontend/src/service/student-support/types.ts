
export interface FAQ {
    id: number;
    question: string;
    answer: string;
}

export interface CreateIssueRequest {
    subject: string;
    description: string;
    categoryId: number;
    userId: string;
}
