export interface StudentRequestDto {
    id: string; 
    date: string; 
    studentIndex: string;
    requestType: string;
    requestDetails: string;
    status: 'Pending' | 'Completed' | 'Rejected';
}

export interface ConfirmationData {
    statusRequest: 'Approved' | 'Rejected';
    shouldPrint: boolean;
}

export interface CreateConfirmationRequest {
    studentIndex: string;
    requestType: string;
    statusRequest: 'Approved' | 'Rejected';
    shouldPrint: boolean;
}

export interface ConfirmationModalProps {
    visible: boolean; 
    onClose: () => void;
    request: StudentRequestDto;
    onSuccess: () => void;
}