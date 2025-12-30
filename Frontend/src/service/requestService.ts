const API_BASE = '/api/Support'; 

// Pomoćna funkcija za dobijanje headera sa tokenom
const getAuthHeaders = () => {
    const token = localStorage.getItem("token"); // Provjeri kako se tvoj ključ tačno zove (npr. 'jwt' ili 'token')
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
            headers: getAuthHeaders() // Koristi funkciju za headere
        });

        if (!response.ok) {
            throw new Error(`Greška: ${response.status}`);
        }
        return await response.json();
    },

    updateStatus: async (requestId: string, status: 'Approved' | 'Rejected'): Promise<any> => {
        const response = await fetch(`${API_BASE}/requests/${requestId}/status`, {
            method: 'PATCH', 
            headers: getAuthHeaders(), // Koristi funkciju za headere
            body: JSON.stringify({ Status: status })
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `Greška pri ažuriranju: ${response.status}`);
        }
        return await response.json();
    }
};