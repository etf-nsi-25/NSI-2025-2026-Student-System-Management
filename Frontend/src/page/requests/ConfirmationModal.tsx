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
    CFormTextarea,
} from '@coreui/react';
import type { StudentRequestDto } from './RequestTypes';
import './ConfirmationModal.css';

interface ConfirmationModalProps {
    visible: boolean;
    onClose: () => void;
    request: StudentRequestDto | null;
    onSuccess: (updatedStatus: 'Approved' | 'Rejected', requestId: string, formData: any) => Promise<void>;
    isProcessing: boolean;
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
    onSuccess,
    isProcessing
}) => {
    
    const [formData, setFormData] = useState<ConfirmationFormData>({
        studentIndex: '',
        requestType: '',
        requestDetails: '',
        statusRequest: '', 
    });
    
    const [validated, setValidated] = useState(false);
    
    useEffect(() => {
        if (visible && request) {
            setFormData({
                studentIndex: request.studentIndex,
                requestType: request.requestType,
                requestDetails: request.requestDetails,
                statusRequest: '', 
            });
            setValidated(false);
        }
    }, [visible, request]);

    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: value as 'Approved' | 'Rejected' | '',
        }));
    };
    
    const handleConfirmation = useCallback(async (event: React.FormEvent, printAction: boolean) => {
        event.preventDefault();
        
        if (!request || !formData.statusRequest) {
            setValidated(true); 
            return;
        }

        try {
            await onSuccess(formData.statusRequest, request.id, {
                ...formData,
                shouldPrint: printAction,
                markStatus: true,
            });
            
        } catch (error) {
            console.error('Confirmation error:', error);
        }
    }, [formData, request, onSuccess]);
    
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

                    <div className="ui-form-field mb-3">
                        <CFormLabel htmlFor="statusRequest" className="ui-field-label">Status Request (Approval)</CFormLabel>
                        <CFormSelect
                            id="statusRequest"
                            name="statusRequest"
                            value={formData.statusRequest}
                            onChange={handleChange}
                            className="form-select bg-white text-dark" 
                            aria-label="Select approval status"
                            required
                            disabled={isProcessing}
                            invalid={validated && !formData.statusRequest}
                        >
                            <option value="">Select Status</option>
                            <option value="Approved">Approved</option>
                            <option value="Rejected">Rejected</option>
                        </CFormSelect>
                        {validated && !formData.statusRequest && (
                            <div className="invalid-feedback d-block">
                                Please select an approval status.
                            </div>
                        )}
                    </div>

                </CModalBody>
                
                <CModalFooter className="justify-content-center border-0 p-4 pt-3">
                    <CButton 
                        color="secondary" 
                        type="button" 
                        onClick={onClose}
                        disabled={isProcessing}
                        className="ui-btn-confirmation ui-btn-secondary-custom me-3"
                    >
                        Cancel
                    </CButton>
                    
                    <CButton 
                        color="primary" 
                        type="submit" 
                        disabled={!isFormValid || isProcessing}
                        className="ui-btn-confirmation ui-btn-primary-custom me-3"
                    >
                        {isProcessing ? 'Processing...' : 'Create Confirmation'}
                    </CButton>
                    
                    <CButton 
                        color="secondary"
                        type="button" 
                        onClick={(e) => handleConfirmation(e, true)} 
                        disabled={!isFormValid || isProcessing}
                        className="ui-btn-confirmation ui-btn-secondary-custom"
                    >
                        {isProcessing ? 'Processing...' : 'Create & Print'}
                    </CButton>
                </CModalFooter>
            </form>
        </CModal>
    );
}

export default ConfirmationModal;