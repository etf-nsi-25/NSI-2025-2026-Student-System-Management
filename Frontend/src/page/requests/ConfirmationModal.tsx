// ConfirmationModal.tsx

import './ConfirmationModal.css';
import React, { useState, useEffect, useCallback } from 'react';
import { 
    CModal, 
    CModalHeader, 
    CModalTitle, 
    CModalBody, 
    CModalFooter, 
    CButton,
    CFormInput,
    CFormSelect,
    CFormLabel,
    CFormFeedback,
    CFormTextarea,
} from '@coreui/react';
import type { StudentRequestDto } from './RequestTypes'; 

interface ConfirmationModalProps {
    visible: boolean;
    onClose: () => void;
    request: StudentRequestDto | null;
    onSuccess: (updatedStatus: 'Approved' | 'Rejected', requestId: string) => void;
}

interface ConfirmationFormData {
    studentIndex: string;
    requestType: string;
    requestDetails: string;
    statusRequest: 'Approved' | 'Rejected' | ''; 
}

const ConfirmationModal: React.FC<ConfirmationModalProps> = ({ 
    visible, 
    onClose, 
    request, 
    onSuccess 
}) => {
    
    const [formData, setFormData] = useState<ConfirmationFormData>({
        studentIndex: '',
        requestType: '',
        requestDetails: '',
        statusRequest: '', 
    });
    
    const [validated, setValidated] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    
    // Initialize form with request data when modal opens
    useEffect(() => {
        if (visible && request) {
            setFormData({
                studentIndex: request.studentIndex,
                requestType: request.requestType,
                requestDetails: request.requestDetails,
                statusRequest: '', 
            });
            setValidated(false);
            setIsSubmitting(false);
        }
    }, [visible, request]);

    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };
    
    const handleConfirmation = useCallback(async (event: React.FormEvent, printAction: boolean) => {
        event.preventDefault();
        
        if (!request || !formData.statusRequest) {
            setValidated(true); 
            return;
        }

        setIsSubmitting(true);

        const confirmationData = {
            studentIndex: formData.studentIndex,   
            requestType: formData.requestType,     
            requestDetails: formData.requestDetails,
            statusRequest: formData.statusRequest,
            shouldPrint: printAction, 
            markStatus: true, 
        };
        
        setValidated(true);

        try {
            const requestId = request.id; 
            const apiUrl = `/api/faculty/requests/${requestId}/confirm`;

            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(confirmationData),
            });
            
            if (!response.ok) {
                const errorBody = await response.text();
                let errorMessage = `Failed to create confirmation (Status: ${response.status}).`;
                try {
                    const errorJson = JSON.parse(errorBody);
                    if (errorJson.errors) {
                        const validationFields = Object.keys(errorJson.errors).join(', ');
                        errorMessage = `Confirmation failed. Missing required fields: ${validationFields}.`;
                    } else if (errorJson.title) {
                        errorMessage = errorJson.title;
                    }
                } catch (e) {
                    // Fallback to generic message if parsing fails
                }
                console.error(`HTTP Error ${response.status}:`, errorBody);
                throw new Error(errorMessage);
            }
            
            onSuccess(formData.statusRequest, requestId);
            
            setTimeout(() => {
                setIsSubmitting(false);
                onClose();
            }, 500);
            
        } catch (error) {
            console.error('Confirmation API error:', error);
            alert(`Failed to process confirmation: ${error instanceof Error ? error.message : String(error)}`);
            setIsSubmitting(false);
        }
    }, [formData, request, onSuccess, onClose]);
    
    const isFormValid = formData.statusRequest !== '';

    return (
        <CModal 
            visible={visible} 
            onClose={onClose} 
            backdrop={true} 
            keyboard={true}
            className="modal-super-high-zindex ui-modal-custom-wrapper" 
            alignment="center" 
            size="lg"
        >
            <CModalHeader className="ui-modal-header-custom p-3">
                <CModalTitle className="ui-modal-title-custom">Create Confirmation</CModalTitle>
            </CModalHeader>
            
            <form onSubmit={(e) => handleConfirmation(e, false)} noValidate className={validated && !isFormValid ? 'needs-validation' : ''}>
                <CModalBody className="p-4">
                    
                    {/* Student Index - Read only */}
                    <div className="ui-form-field mb-3">
                        <CFormLabel htmlFor="studentIndex" className="ui-field-label">Student Index</CFormLabel>
                        <CFormInput 
                            id="studentIndex" 
                            name="studentIndex"
                            value={formData.studentIndex} 
                            readOnly
                            className="ui-input-base bg-light text-dark" 
                        />
                    </div>
                    
                    {/* Request Type - Read only */}
                    <div className="ui-form-field mb-3">
                        <CFormLabel htmlFor="requestType" className="ui-field-label">Request Type</CFormLabel>
                        <CFormInput
                            id="requestType"
                            name="requestType"
                            value={formData.requestType}
                            readOnly
                            className="ui-input-base bg-light text-dark"
                            aria-label="Request type"
                        />
                    </div>

                    {/* Request Details - Read only textarea */}
                    <div className="ui-form-field mb-3">
                        <CFormLabel htmlFor="requestDetails" className="ui-field-label">Request Details</CFormLabel>
                        <CFormTextarea
                            id="requestDetails"
                            name="requestDetails"
                            value={formData.requestDetails}
                            readOnly
                            className="ui-input-base bg-light text-dark"
                            rows={4}
                            aria-label="Request details"
                            style={{ 
                                resize: 'none',
                                minHeight: '100px'
                            }}
                        />
                    </div>

                    {/* Status Request Dropdown (Approved/Rejected) */}
                    <div className="ui-form-field mb-3">
                        <CFormLabel htmlFor="statusRequest" className="ui-field-label">Status Request (Approval)</CFormLabel>
                        <CFormSelect
                            id="statusRequest"
                            name="statusRequest"
                            value={formData.statusRequest}
                            onChange={(e) => handleChange(e as React.ChangeEvent<HTMLSelectElement>)}
                            className="form-select bg-white text-dark" 
                            aria-label="Select approval status"
                            required
                            disabled={isSubmitting}
                        >
                            <option value="">Select Status</option>
                            <option value="Approved">Approved</option>
                            <option value="Rejected">Rejected</option>
                        </CFormSelect>
                        <CFormFeedback invalid>Please select an approval status.</CFormFeedback>
                    </div>

                </CModalBody>
                
                {/* Footer with action buttons */}
                <CModalFooter className="justify-content-center border-0 p-4 pt-3">
                    {/* Create Confirmation button */}
                    <CButton 
                        color="primary" 
                        type="submit" 
                        disabled={!isFormValid || isSubmitting}
                        className="ui-btn-confirmation ui-btn-primary-custom me-3"
                    >
                        {isSubmitting ? 'Processing...' : 'Create Confirmation'}
                    </CButton>
                    
                    {/* Create & Print button */}
                    <CButton 
                        color="secondary"
                        type="button" 
                        onClick={(e) => handleConfirmation(e, true)} 
                        disabled={!isFormValid || isSubmitting}
                        className="ui-btn-confirmation ui-btn-secondary-custom"
                    >
                        {isSubmitting ? 'Processing...' : 'Create & Print'}
                    </CButton>
                </CModalFooter>
            </form>
        </CModal>
    );
}

export default ConfirmationModal;