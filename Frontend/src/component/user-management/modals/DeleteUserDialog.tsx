import type { User } from "../../../types/user-types";
import Modal from "../../shared/Modal";
import { CButton } from '@coreui/react';

interface DeleteConfirmDialogProps {
  user: User | null;
  onConfirm: () => void;
  onCancel: () => void;
}

export default function DeleteConfirmDialog({ user, onConfirm, onCancel }: DeleteConfirmDialogProps) {
  return (
    <Modal onClose={onCancel}>
      <div className="text-center">
        <h2 className="h5 text-danger mb-3">Delete User</h2>
        <p className="mb-4">
          Are you sure you want to delete <span className="fw-semibold">{user?.username}</span>? This action cannot be undone.
        </p>
        <div className="d-flex justify-content-center gap-2">
          <CButton color="secondary" onClick={onCancel}>Cancel</CButton>
          <CButton color="danger" onClick={onConfirm}>Delete</CButton>
        </div>
      </div>
    </Modal>
  );
}
