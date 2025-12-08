import { useState } from "react";
import type { User } from "../../../types/user-types";
import Modal from "../../Modal";

interface EditUserModalProps {
  user: User;
  onClose: () => void;
  onSave: (user: User) => void;
}

export default function EditUserModal({ user, onClose, onSave }: EditUserModalProps) {
  const [formData, setFormData] = useState(user);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.name || !formData.email) {
      alert('Please fill all required fields');
      return;
    }
    onSave(formData);
  };

  return (
    <Modal onClose={onClose}>
      <h2 className="text-lg font-bold text-blue-900 mb-6">Edit User</h2>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-semibold mb-2">Full Name:</label>
          <input
            type="text"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-semibold mb-2">Email Address:</label>
          <input
            type="email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-semibold mb-2">Role:</label>
          <select
            value={formData.role}
            onChange={(e) => setFormData({ ...formData, role: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
          >
            <option>Professor</option>
            <option>Assistant</option>
            <option>Student</option>
            <option>Staff</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-semibold mb-2">Faculty:</label>
          <select
            value={formData.faculty}
            onChange={(e) => setFormData({ ...formData, faculty: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
          >
            <option>ETF UNSA</option>
          </select>
        </div>

        <div className="flex gap-3 justify-end mt-6">
          <button
            type="button"
            onClick={onClose}
            className="bg-gray-300 text-gray-900 px-6 py-2 rounded text-sm font-semibold hover:bg-gray-400"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="bg-blue-900 text-white px-6 py-2 rounded text-sm font-semibold hover:bg-blue-800"
          >
            Save
          </button>
        </div>
      </form>
    </Modal>
  );
}
