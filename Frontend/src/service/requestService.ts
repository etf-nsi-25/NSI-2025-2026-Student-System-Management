const API_BASE = '/api/Support'; 

const getAuthHeaders = () => {
    const token = localStorage.getItem("token"); 
    const headers: HeadersInit = {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    };
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }
    return headers;
};

export const requestService = {
    getAllRequests: async () => {
        const response = await fetch(`${API_BASE}/requests`, {
            method: 'GET',
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            throw new Error(`Error: ${response.status}`);
        }
        return await response.json();
    },

    updateStatus: async (requestId: string, status: 'Approved' | 'Rejected'): Promise<any> => {
        const response = await fetch(`${API_BASE}/requests/${requestId}/status`, {
            method: 'PATCH', 
            headers: getAuthHeaders(), 
            body: JSON.stringify({ Status: status })
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `Update errror: ${response.status}`);
        }
        return await response.json();
    }
};