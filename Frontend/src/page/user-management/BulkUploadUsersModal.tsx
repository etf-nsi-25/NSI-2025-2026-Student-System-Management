import React, { useState } from "react";
import { CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter, CButton, CFormLabel, CFormInput, CTable, CTableHead, CTableRow, CTableHeaderCell, CTableBody, CTableDataCell } from "@coreui/react";

type BulkUserPreview = {
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  faculty: string;
  valid: boolean;
  error?: string;
};

const MOCK_PREVIEW_USERS: BulkUserPreview[] = [
  {
    firstName: "Amina",
    lastName: "Hadžić",
    email: "amina.hadzic@unsa.ba",
    role: "Student",
    faculty: "Faculty of Electrical Engineering",
    valid: true,
  },
  {
    firstName: "Marko",
    lastName: "Marić",
    email: "marko.maric@unsa.ba",
    role: "Student",
    faculty: "Faculty of Electrical Engineering",
    valid: true,
  },
  {
    firstName: "Lejla",
    lastName: "Kovač",
    email: "lejla.kovac@unsa",
    role: "Student",
    faculty: "Faculty of Electrical Engineering",
    valid: false,
    error: "Invalid email format",
  },
];

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export default function BulkUploadUsersModal({
  isOpen,
  onClose,
  onSuccess,
}: Props) {
  const [step, setStep] = useState<"upload" | "preview">("upload");
  const [file, setFile] = useState<File | null>(null);
  const [previewUsers, setPreviewUsers] = useState<BulkUserPreview[]>([]);
  const [error, setError] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setError(null);
    const selected = e.target.files?.[0] || null;
    setFile(selected);
  };

  const handleUploadMock = () => {
    if (!file) {
      setError("Please select a file.");
      return;
    }

    setPreviewUsers(MOCK_PREVIEW_USERS);
    setStep("preview");
  };

  const handleConfirmMock = () => {
    onSuccess();
    resetAndClose();
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
              Required columns: firstName, lastName, email, role, faculty
            </small>
          </>
        )}

        {step === "preview" && (
          <>
            <CTable small bordered responsive>
              <CTableHead>
                <CTableRow>
                  <CTableHeaderCell>Name</CTableHeaderCell>
                  <CTableHeaderCell>Email</CTableHeaderCell>
                  <CTableHeaderCell>Role</CTableHeaderCell>
                  <CTableHeaderCell>Faculty</CTableHeaderCell>
                  <CTableHeaderCell>Status</CTableHeaderCell>
                </CTableRow>
              </CTableHead>

              <CTableBody>
                {previewUsers.map((u, idx) => (
                  <CTableRow
                    key={idx}
                    className={!u.valid ? "table-danger" : ""}
                  >
                    <CTableDataCell>
                      {u.firstName} {u.lastName}
                    </CTableDataCell>
                    <CTableDataCell>{u.email}</CTableDataCell>
                    <CTableDataCell>{u.role}</CTableDataCell>
                    <CTableDataCell>{u.faculty}</CTableDataCell>
                    <CTableDataCell>
                      {u.valid ? (
                        <span className="text-success">Valid</span>
                      ) : (
                        <span className="text-danger">
                          Invalid – {u.error}
                        </span>
                      )}
                    </CTableDataCell>
                  </CTableRow>
                ))}
              </CTableBody>
            </CTable>

            <small className="text-muted">
              Invalid rows will not be imported.
            </small>
          </>
        )}
      </CModalBody>

      <CModalFooter>
        {step === "upload" && (
          <>
            <CButton color="primary" onClick={handleUploadMock}>
              Upload
            </CButton>
            <CButton color="secondary" onClick={resetAndClose}>
              Cancel
            </CButton>
          </>
        )}

        {step === "preview" && (
          <>
            <CButton color="primary" onClick={handleConfirmMock}>
              Confirm import
            </CButton>
            <CButton
              color="secondary"
              onClick={() => setStep("upload")}
            >
              Back
            </CButton>
          </>
        )}
      </CModalFooter>
    </CModal>
  );
}
