import React, { useState } from "react";
import { CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter, CButton, CFormLabel, CFormInput, CTable, CTableHead, CTableRow, CTableHeaderCell, CTableBody, CTableDataCell } from "@coreui/react";
import { useAPI } from "../../context/services";

type StudentImportPreview = {
    firstName: string;
    lastName: string;
    username: string;
    email?: string;
    indexNumber: string;
    errors: string[];
};

interface Props {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
}

export default function BulkUploadUsersModal({isOpen, onClose, onSuccess,}: Props) {
    const api = useAPI();   
    
    const [step, setStep] = useState<"upload" | "preview">("upload");
    const [file, setFile] = useState<File | null>(null);
    const [previewUsers, setPreviewUsers] = useState<StudentImportPreview[]>([]);
    const [error, setError] = useState<string | null>(null);

    const isDisabled = previewUsers.length == 0 || previewUsers.some((u) => u.errors.length > 0) 

    if (!isOpen) return null;

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setError(null);
        const selected = e.target.files?.[0] || null;
        setFile(selected);
    };

    const handleUpload = async () => {
    if (!file) {
        setError("Please select a file.");
        return;
    }

    try {
        const formData = new FormData();
        formData.append("file", file);

        const data = await api.post<StudentImportPreview[]>("/api/faculty/students/import/preview", formData);

        setPreviewUsers(data);
        setStep("preview");
    } catch (e: any) {
        setError(e.message);
    }
    };    

    const handleConfirm = async () => {
        try {
            const validStudents = previewUsers.filter((u) => u.errors.length === 0);

            await api.post("/api/faculty/students/import/commit", validStudents);

            onSuccess();
            resetAndClose();
        } catch (e: any) {
            setError(e.message);
        }
    };

    const resetAndClose = () => {
        setStep("upload");
        setFile(null);
        setPreviewUsers([]);
        setError(null);
        onClose();
    };

    return (
        <CModal
        visible={isOpen}
        onClose={resetAndClose}
        size="xl"
        alignment="center"
        className="modal-super-high-zindex modal-form bulk-preview-modal"
        backdrop="static"
        >
        <CModalHeader closeButton>
            <CModalTitle>
            {step === "upload" ? "Import users (CSV / Excel)" : "Preview users"}
            </CModalTitle>
        </CModalHeader>

        <CModalBody>
            {step === "upload" && (
            <>
                <div className="mb-3">
                <CFormLabel htmlFor="bulkFile">Upload file</CFormLabel>
                <CFormInput
                    id="bulkFile"
                    type="file"
                    accept=".csv,.xlsx,.xls"
                    onChange={handleFileChange}
                />
                </div>

                {error && (
                <div className="text-danger small mb-2">{error}</div>
                )}

                <small className="text-muted">
                Supported formats: CSV, XLSX <br />
                Required columns: firstName, lastName, username, email, index
                </small>
            </>
            )}

            {step === "preview" && (
            <>
                <CTable small bordered responsive>
                <CTableHead>
                    <CTableRow>
                    <CTableHeaderCell>Name</CTableHeaderCell>
                    <CTableHeaderCell>Username</CTableHeaderCell>
                    <CTableHeaderCell>Email</CTableHeaderCell>
                    <CTableHeaderCell>Index</CTableHeaderCell>
                    <CTableHeaderCell>Status</CTableHeaderCell>
                    </CTableRow>
                </CTableHead>

                <CTableBody>
                    {previewUsers.map((u, idx) => (
                    <CTableRow
                        key={idx}
                        className={u.errors.length ? "table-danger" : ""}
                    >
                        <CTableDataCell>
                        {u.firstName} {u.lastName}
                        </CTableDataCell>
                        <CTableDataCell>{u.username}</CTableDataCell>
                        <CTableDataCell>{u.email}</CTableDataCell>
                        <CTableDataCell>{u.indexNumber}</CTableDataCell>
                        <CTableDataCell>
                        {u.errors.length === 0 ? (
                            <span className="text-success">Valid</span>
                        ) : (
                            <span className="text-danger">
                            Invalid – {u.errors.join(", ")}
                            </span>
                        )}
                        </CTableDataCell>
                    </CTableRow>
                    ))}
                </CTableBody>
                </CTable>
            </>
            )}
        </CModalBody>

        <CModalFooter>
            {step === "upload" && (
            <>
                <CButton color="primary" onClick={handleUpload}>
                    Upload
                </CButton>
                <CButton color="secondary" onClick={resetAndClose}>
                    Cancel
                </CButton>
            </>
            )}

            {step === "preview" && (
            <>
                <CButton color="primary" onClick={handleConfirm} disabled={isDisabled}>
                    Confirm import
                </CButton>
                <CButton color="secondary" onClick={() => setStep("upload")}>
                    Back
                </CButton>
            </>
            )}
        </CModalFooter>
        </CModal>
    );
}