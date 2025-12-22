import axios from 'axios';

const API_BASE = 'https://localhost:5283/api/Support';

export const requestService = {
    getAllRequests: async () => {
        const response = await axios.get(`${API_BASE}/requests`);
        return response.data;
    },

    updateStatus: async (requestId: string, status: 'Approved' | 'Rejected'): Promise<any> => {
        
        const response = await axios.patch(
            `${API_BASE}/requests/${requestId}/status`,
            { 
                Status: status 
            }
        );
        return response.data;
    }
};