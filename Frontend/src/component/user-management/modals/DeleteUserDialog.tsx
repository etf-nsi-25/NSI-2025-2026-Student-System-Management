import type { User } from "../../../types/user-types";
import Modal from "../../Modal";

interface DeleteConfirmDialogProps {
  user: User | null;
  onConfirm: () => void;
  onCancel: () => void;
}

export default function DeleteConfirmDialog({ user, onConfirm, onCancel }: DeleteConfirmDialogProps) {
  return (
    <Modal onClose={onCancel}>
      <div className="text-center">
        <h2 className="text-lg font-bold text-red-600 mb-4">Delete User</h2>
        <p className="text-gray-700 mb-6">
          Are you sure you want to delete <span className="font-semibold">{user?.name}</span>? This action cannot be undone.
        </p>
        <div className="flex gap-3 justify-center">
          <button
            onClick={onCancel}
            className="bg-gray-300 text-gray-900 px-6 py-2 rounded text-sm font-semibold hover:bg-gray-400"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="bg-red-600 text-white px-6 py-2 rounded text-sm font-semibold hover:bg-red-700"
          >
            Delete
          </button>
        </div>
      </div>
    </Modal>
  );
}
