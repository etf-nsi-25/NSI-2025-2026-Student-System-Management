export const parseDate = (dateValue: any): Date => {
    if (dateValue instanceof Date) {
        return dateValue;
    }
    if (typeof dateValue === 'string') {
        return new Date(dateValue);
    }
    if (typeof dateValue === 'number') {
        return new Date(dateValue);
    }
    return new Date();
};

export type RequestStatus = 'Pending' | 'Approved' | 'Rejected'; 

export const renderStatus = (status: RequestStatus): 'success' | 'danger' | 'warning' => {
    switch (status) {
        case 'Approved':  
            return 'success';
        case 'Rejected':
            return 'danger';
        case 'Pending':
            return 'warning';
        default:
            return 'warning';
    }
};