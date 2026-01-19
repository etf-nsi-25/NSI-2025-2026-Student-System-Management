import React from "react";
import {
    CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter,
    CButton, CRow, CCol
} from '@coreui/react';
import type { User } from "../../service/identityApi"; // <--- ISPRAVLJENO


interface UserDetailsModalProps {
    isOpen: boolean;
    onClose: () => void;
    user: User | null;
    onEditUser: (user: User) => void;
}

// Mock Podaci za dozvole
const MOCK_PERMISSIONS: { [key in User['role']]: string } = {
    Professor: "Edit Courses, Grade Students",
    Assistant: "View Courses, Grade Students",
    Student: "View Grades, Enroll in Courses",
    Admin: "Manage University Data",
    Superadmin: "Manage All System Data",
};

const UserDetailsModal: React.FC<UserDetailsModalProps> = ({
    isOpen,
    onClose,
    user,
    onEditUser
}) => {
    if (!isOpen || !user) return null;

    const getFormattedName = (u: User) => {
        const prefix = u.role === "Professor" ? "Prof." : "";
        return `${prefix} ${u.firstName} ${u.lastName}`;
    };

    const handleEditClick = () => {
        onEditUser(user);
    };

    return (
        <CModal visible={isOpen} onClose={onClose} size="sm">
            <CModalHeader closeButton>
                <CModalTitle>User Details</CModalTitle>
            </CModalHeader>
            <CModalBody>

                {/* Prikaz detalja u Core UI stilu (CRow i CCol za layout) */}
                <CRow className="mb-2">
                    <CCol xs={5} className="font-weight-bold text-muted">Name:</CCol>
                    <CCol xs={7}>{getFormattedName(user)}</CCol>
                </CRow>

                <CRow className="mb-2">
                    <CCol xs={5} className="font-weight-bold text-muted">Role:</CCol>
                    <CCol xs={7}>{user.role}</CCol>
                </CRow>

                <CRow className="mb-2">
                    <CCol xs={5} className="font-weight-bold text-muted">Faculty:</CCol>
                    <CCol xs={7}>{user.facultyId}</CCol>
                </CRow>

                {user.role === 'Student' && user.indexNumber && (
                    <CRow className="mb-2">
                        <CCol xs={5} className="font-weight-bold text-muted">Index:</CCol>
                        <CCol xs={7}>{user.indexNumber}</CCol>
                    </CRow>
                )}

                <CRow className="mb-2">
                    <CCol xs={5} className="font-weight-bold text-muted">Permissions:</CCol>
                    <CCol xs={7}>{MOCK_PERMISSIONS[user.role]}</CCol>
                </CRow>

                <CRow className="mb-2">
                    <CCol xs={5} className="font-weight-bold text-muted">Status:</CCol>
                    <CCol xs={7}>{user.status}</CCol>
                </CRow>

                <CRow className="mb-2">
                    <CCol xs={5} className="font-weight-bold text-muted">Last login:</CCol>
                    <CCol xs={7}>{user.lastActive}</CCol>
                </CRow>

            </CModalBody>
            <CModalFooter>
                <CButton color="primary" onClick={handleEditClick}>
                    Edit Permissions
                </CButton>
                <CButton color="secondary" onClick={onClose}>
                    Close
                </CButton>
            </CModalFooter>
        </CModal>
    );
};

export default UserDetailsModal;