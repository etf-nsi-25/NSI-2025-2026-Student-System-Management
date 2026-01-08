import React from 'react';
import {
    CModal,
    CModalHeader,
    CModalTitle,
    CModalBody,
    CModalFooter,
    CButton,
} from '@coreui/react';
import type { StudentRequestDto } from './RequestTypes'; 
import './RequestDetailsModal.css';
import { parseDate } from '../../utils/requestUtils'; 
import StatusBadge from '../../component/StatusBadge'; 

interface RequestDetailsModalProps {
    visible: boolean;
    onClose: () => void;
    request: StudentRequestDto | null;
}

const RequestDetailsModal: React.FC<RequestDetailsModalProps> = ({ 
    visible, 
    onClose, 
    request 
}) => {
    if (!request) return null;

    const requestDate = parseDate(request.date);

    return (
        <CModal 
            visible={visible} 
            onClose={onClose}
            backdrop={true}
            keyboard={true}
            size="lg"
            alignment="center"
            className="request-details-modal"
        >
            <CModalHeader className="request-details-modal-header">
                <CModalTitle className="text-white">Request Details</CModalTitle>
            </CModalHeader>
            <CModalBody>
                <div className="row">
                    <div className="col-md-6 mb-3">
                        <strong>Date Submitted:</strong>
                        <p className="text-muted">{requestDate.toLocaleDateString()}</p>
                    </div>
                    <div className="col-md-6 mb-3">
                        <strong>Student Index:</strong>
                        <p className="text-muted">{request.studentIndex}</p>
                    </div>
                    <div className="col-12 mb-3">
                        <strong>Request Type:</strong>
                        <p className="text-muted">{request.requestType}</p>
                    </div>
                    <div className="col-12 mb-3">
                        <strong>Request Details:</strong>
                        <div className="request-details-box">
                            {request.requestDetails || "No additional details provided."}
                        </div>
                    </div>
                    <div className="col-12 mb-3">
                        <strong>Status:</strong>
                        <div className="mt-1 d-inline-block">
                            <StatusBadge status={request.status} />
                        </div>
                    </div>
                </div>
            </CModalBody>
            <CModalFooter>
                <CButton 
                    color="secondary" 
                    onClick={onClose}
                    className="request-details-close-btn"
                >
                    Close
                </CButton>
            </CModalFooter>
        </CModal>
    );
};

export default RequestDetailsModal;