// RequestManagement.tsx
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
    CBadge,
    CInputGroup,
    CInputGroupText,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPlus, cilDescription, cilOptions, cilZoom } from '@coreui/icons';
import ConfirmationModal from './ConfirmationModal'; 
import RequestDetailsModal from './RequestDetailsModal';
import type { StudentRequestDto } from './RequestTypes'; 
import './RequestManagement.css'; 

const API_BASE_URL = '/api/faculty/requests';

// Helper function to safely parse date
const parseDate = (dateValue: any): Date => {
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

// Status rendering helper
const renderStatus = (status: StudentRequestDto['status']) => {
    let color: 'success' | 'danger' | 'info' | 'warning' = 'info'; 
    switch (status) {
        case 'Completed':
            color = 'success';
            break; 
        case 'Rejected':
            color = 'danger';
            break; 
        default:
            color = 'warning'; // 'Pending'
    }
    return (
        <CBadge 
            color={color} 
            className="text-white py-2 px-3 rounded-pill" 
        >
            {status}
        </CBadge>
    );
};

// Action buttons rendering helper
const renderActions = (
    request: StudentRequestDto, 
    handleOpenModal: (req: StudentRequestDto) => void,
    handleViewDetails: (req: StudentRequestDto) => void,
    handleMoreActions: (req: StudentRequestDto) => void
) => {
    
    const isConfirmationAllowed = request.status === 'Pending';

    return (
        <div className="d-flex justify-content-center gap-2">
            {/* Create Confirmation button */}
            <CButton
                color="transparent" 
                className="p-1 ui-btn-action ui-btn-plus" 
                onClick={() => handleOpenModal(request)}
                disabled={!isConfirmationAllowed}
                title={isConfirmationAllowed ? "Create Confirmation" : "Cannot create confirmation for processed requests"}
            >
                <CIcon icon={cilPlus} />
            </CButton>

            {/* View Details button */}
            <CButton
                color="transparent" 
                className="p-1 ui-btn-action ui-btn-view" 
                onClick={() => handleViewDetails(request)}
                title="View Request Details"
            >
                <CIcon icon={cilDescription} />
            </CButton>

            {/* More Actions button */}
            <CButton
                color="transparent" 
                className="p-1 ui-btn-action"
                onClick={() => handleMoreActions(request)}
                title="More Actions"
            >
                <CIcon icon={cilOptions} />
            </CButton>
        </div>
    );
};

// Main component
export function RequestManagement() {
    
    const [requests, setRequests] = useState<StudentRequestDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState<boolean>(false);
    const [isDetailsModalOpen, setIsDetailsModalOpen] = useState<boolean>(false);
    const [selectedRequest, setSelectedRequest] = useState<StudentRequestDto | null>(null);
    const [searchTerm, setSearchTerm] = useState<string>('');

    const fetchRequests = useCallback(async () => {
        setIsLoading(true);
        setError(null);
        try {
            const response = await fetch(API_BASE_URL);

            if (!response.ok) {
                throw new Error(`Failed to fetch requests: ${response.statusText}`);
            }

            const data: StudentRequestDto[] = await response.json();
            setRequests(data);
        } catch (err: any) {
            console.error('API Error:', err);
            setError(err.message || 'An unknown error occurred while fetching requests.');
        } finally {
            setIsLoading(false);
        }
    }, []);

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
        console.log('More actions for request:', request.id);
        alert(`More actions for request: ${request.id}\n\nAvailable options:\n1. Download PDF\n2. Send Reminder\n3. Archive Request\n4. View History`);
    };

    const handleConfirmationSuccess = useCallback((newStatus: 'Approved' | 'Rejected', requestId: string) => {
        setRequests(prevRequests => 
            prevRequests.map(request => {
                if (request.id === requestId) {
                    const mappedStatus = newStatus === 'Approved' ? 'Completed' : 'Rejected';
                    const updatedDate = new Date().toISOString();
                    
                    return {
                        ...request,
                        status: mappedStatus,
                        date: updatedDate
                    };
                }
                return request;
            })
        );
    }, []);

    const filteredRequests = useMemo(() => {
        if (!searchTerm) {
            return requests;
        }

        const lowerCaseSearchTerm = searchTerm.toLowerCase().trim();

        return requests.filter(request => {
            return (
                request.studentIndex.toLowerCase().includes(lowerCaseSearchTerm) ||
                request.requestType.toLowerCase().includes(lowerCaseSearchTerm) ||
                request.requestDetails.toLowerCase().includes(lowerCaseSearchTerm)
            );
        });
    }, [requests, searchTerm]);

    const columns = [
        'Date',
        'Student index',
        'Request type',
        'Request details',
        'Status',
        'Actions',
    ];

    return (
        <>
            {/* Main Card Container */}
            <CCard className="shadow-lg border-0 ui-card-custom"> 
                
                {/* Card Header: Title and Search */}
                <CCardHeader className="d-flex flex-wrap justify-content-between align-items-center bg-transparent border-0 pt-4 px-4 pb-3">
                    <h2 className="mb-2 mb-sm-0 text-primary fw-bold" style={{ fontSize: '2rem' }}>Request management</h2>
                    
                    <CInputGroup className="ui-search-input-group">
                        <CFormInput
                            type="text"
                            placeholder="Search by index, type or details..." 
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="ui-search-input" 
                        />
                        <CInputGroupText className="ui-search-icon-bg">
                            <CIcon icon={cilZoom} className="ui-search-icon" />
                        </CInputGroupText>
                    </CInputGroup>
                </CCardHeader>
                
                {/* Card Body: Table */}
                <CCardBody className="p-4">
                    {isLoading ? (
                        <p className="text-center text-muted">Loading requests...</p>
                    ) : error ? (
                        <p className="text-center text-danger">{error}</p>
                    ) : filteredRequests.length === 0 ? (
                        <div className="text-center py-5">
                            <CIcon icon={cilDescription} size="xl" className="text-muted mb-3" />
                            <h5 className="text-muted mb-3">No Requests Found</h5>
                            <p className="text-muted">
                                {searchTerm 
                                    ? 'No requests match your search criteria. Try a different search term.'
                                    : 'There are no student requests currently available in the system.'}
                            </p>
                        </div>
                    ) : (
                        <CTable 
                            hover 
                            responsive 
                            className="align-middle mb-0 ui-table-custom"
                        >
                            <CTableHead>
                                <CTableRow>
                                    {columns.map((col, index) => (
                                        <CTableHeaderCell 
                                            key={index} 
                                            className={`text-nowrap ${col === 'Status' || col === 'Actions' ? 'text-center' : ''}`}
                                        >
                                            {col}
                                        </CTableHeaderCell>
                                    ))}
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {filteredRequests.map((request) => {
                                    const requestDate = parseDate(request.date);
                                    
                                    return (
                                        <CTableRow key={request.id}>
                                            <CTableDataCell className="text-nowrap">
                                                {requestDate.toLocaleDateString()}
                                            </CTableDataCell>
                                            <CTableDataCell className="font-weight-bold text-nowrap">
                                                {request.studentIndex}
                                            </CTableDataCell>
                                            <CTableDataCell className="text-nowrap">
                                                {request.requestType}
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <span className="d-inline-block text-truncate" style={{ maxWidth: '300px' }}>
                                                    {request.requestDetails}
                                                </span>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                {renderStatus(request.status)}
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center text-nowrap">
                                                {renderActions(
                                                    request, 
                                                    handleOpenConfirmationModal,
                                                    handleViewDetails,
                                                    handleMoreActions
                                                )}
                                            </CTableDataCell>
                                        </CTableRow>
                                    );
                                })}
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>

            {/* Confirmation Modal */}
            {isConfirmationModalOpen && selectedRequest && (
                <ConfirmationModal
                    visible={isConfirmationModalOpen}
                    onClose={() => setIsConfirmationModalOpen(false)}
                    request={selectedRequest} 
                    onSuccess={handleConfirmationSuccess}
                />
            )}

            {/* Request Details Modal */}
            <RequestDetailsModal
                visible={isDetailsModalOpen}
                onClose={() => setIsDetailsModalOpen(false)}
                request={selectedRequest}
            />
        </>
    );
}

export default RequestManagement;