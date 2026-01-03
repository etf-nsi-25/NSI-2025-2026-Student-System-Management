import { useState, useEffect, useCallback, useMemo } from 'react';
import {
    CCard,
    CCardBody,
    CCardHeader,
    CTable,
    CTableHead,
    CTableBody,
    CTableRow,
    CTableHeaderCell,
    CTableDataCell,
    CButton,
    CFormInput,
    CInputGroup,
    CInputGroupText,
    CSpinner
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPlus, cilDescription, cilOptions, cilZoom } from '@coreui/icons';
import ConfirmationModal from './ConfirmationModal';
import RequestDetailsModal from './RequestDetailsModal';
import type { StudentRequestDto } from './RequestTypes'; 
import './RequestManagement.css';
import { parseDate } from '../../utils/requestUtils';
import StatusBadge from '../../component/StatusBadge';
import { useAPI } from '../../context/services';

export function RequestManagement() {
    const api = useAPI()
    const [requests, setRequests] = useState<StudentRequestDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState<boolean>(false);
    const [isDetailsModalOpen, setIsDetailsModalOpen] = useState<boolean>(false);
    const [selectedRequest, setSelectedRequest] = useState<StudentRequestDto | null>(null);
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [isProcessing, setIsProcessing] = useState<boolean>(false);

    const fetchRequests = useCallback(async () => {
        setIsLoading(true);
        try {
            const data = await api.getAllRequests();
            setRequests(data);
        } catch (err) {
            console.error('Error fetching requests:', err);
        } finally {
            setIsLoading(false);
        }
    }, [api]);

    useEffect(() => {
        fetchRequests();
    }, [fetchRequests]);

    const handleOpenConfirmationModal = (request: StudentRequestDto) => {
        setSelectedRequest(request);
        setIsConfirmationModalOpen(true);
    };

    const handleViewDetails = (request: StudentRequestDto) => {
        setSelectedRequest(request);
        setIsDetailsModalOpen(true);
    };

    const handleMoreActions = (request: StudentRequestDto) => {
        console.log(`Actions for: ${request.id}`);
    };

    const handleConfirmationSuccess = useCallback(async (
        newStatus: 'Approved' | 'Rejected',
        requestId: string  
    ) => {
        setIsProcessing(true);
        try {
            await api.updateStatus(requestId, newStatus);
            
            setIsConfirmationModalOpen(false);
            await fetchRequests(); 
        } catch (err) {
            console.error('Error fetching requests:', err);
        } finally {
            setIsProcessing(false);
        }
    }, [api, fetchRequests]);

    const filteredRequests = useMemo(() => {
        const lowerCaseSearchTerm = searchTerm.toLowerCase().trim();
        return requests.filter(request => 
            request.studentIndex.toLowerCase().includes(lowerCaseSearchTerm) ||
            request.requestType.toLowerCase().includes(lowerCaseSearchTerm)
        );
    }, [requests, searchTerm]);

    const renderActions = (request: StudentRequestDto) => {
        const isConfirmationAllowed = request.status === 'Pending';
        return (
            <div className="d-flex justify-content-center gap-2">
                <CButton 
                    color="transparent" 
                    className="p-1 ui-btn-action ui-btn-plus"
                    onClick={() => handleOpenConfirmationModal(request)}
                    disabled={!isConfirmationAllowed}
                >
                    <CIcon icon={cilPlus} />
                </CButton>
                <CButton 
                    color="transparent" 
                    className="p-1 ui-btn-action ui-btn-view"
                    onClick={() => handleViewDetails(request)}
                >
                    <CIcon icon={cilDescription} />
                </CButton>
                <CButton 
                    color="transparent" 
                    className="p-1 ui-btn-action"
                    onClick={() => handleMoreActions(request)}
                >
                    <CIcon icon={cilOptions} />
                </CButton>
            </div>
        );
    };

    return (
        <>
            <CCard className="shadow-lg border-0">
                <CCardHeader className="bg-white py-3 border-0">
                    <div className="d-flex justify-content-between align-items-center">
                        <h5 className="mb-0 fw-bold text-primary">Student Requests</h5>
                        <CInputGroup style={{ maxWidth: '300px' }}>
                            <CInputGroupText className="bg-light border-end-0">
                                <CIcon icon={cilZoom} />
                            </CInputGroupText>
                            <CFormInput
                                className="bg-light border-start-0"
                                placeholder="Search..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </CInputGroup>
                    </div>
                </CCardHeader>
                <CCardBody>
                    {isLoading ? (
                        <div className="text-center py-5">
                            <CSpinner color="primary" />
                        </div>
                    ) : (
                        <CTable align="middle" responsive hover borderless className="mb-0">
                            <CTableHead className="bg-light text-muted small text-uppercase">
                                <CTableRow>
                                    <CTableHeaderCell>Date</CTableHeaderCell>
                                    <CTableHeaderCell>Student Index</CTableHeaderCell>
                                    <CTableHeaderCell>Request Type</CTableHeaderCell>
                                    <CTableHeaderCell>Status</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">Actions</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {filteredRequests.map((req) => (
                                    <CTableRow key={req.id} className="border-bottom">
                                        <CTableDataCell>{parseDate(req.date).toLocaleDateString()}</CTableDataCell>
                                        <CTableDataCell className="fw-semibold">{req.studentIndex}</CTableDataCell>
                                        <CTableDataCell>{req.requestType}</CTableDataCell>
                                        <CTableDataCell>
                                            <StatusBadge status={req.status} />
                                        </CTableDataCell>
                                        <CTableDataCell>
                                            {renderActions(req)}
                                        </CTableDataCell>
                                    </CTableRow>
                                ))}
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>

            {isConfirmationModalOpen && selectedRequest && (
                <ConfirmationModal
                    visible={isConfirmationModalOpen}
                    onClose={() => setIsConfirmationModalOpen(false)}
                    request={selectedRequest} 
                    onSuccess={handleConfirmationSuccess}
                    isProcessing={isProcessing}
                />
            )}

            <RequestDetailsModal
                visible={isDetailsModalOpen}
                onClose={() => setIsDetailsModalOpen(false)}
                request={selectedRequest}
            />
        </>
    );
}

export default RequestManagement;