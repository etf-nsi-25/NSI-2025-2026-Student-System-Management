import { useState } from 'react';

import Modal from '../../Modal';


interface AddUserModalProps {
  onClose: () => void;
  onAdd: any;
}

export default function AddUserModal({ onClose, onAdd }: AddUserModalProps) {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    username: '',
    password: '',
    indexNumber: '',
    role: 'Professor',
    faculty: 'ETF UNSA',
    lastActive: 'Today',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.username || !formData.password) {
      alert('Please fill all required fields');
      return;
    }
    onAdd(formData);
  };

  return (
    <Modal onClose={onClose}>
      <h2 className="text-lg font-bold text-blue-900 mb-6">Add User</h2>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-semibold mb-2">First Name:</label>
          <input
            type="text"
            value={formData.firstName}
            onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-semibold mb-2">Last Name:</label>
          <input
            type="text"
            value={formData.lastName}
            onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
            required
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-2">Username:</label>
          <input
            type="text"
            value={formData.username}
            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
            required
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-2">Password:</label>
          <input
            type="password"
            value={formData.password}
            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
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
        {formData.role === 'Student' && (
          <div>
            <label className="block text-sm font-semibold mb-2">Index Number:</label>
            <input
              type="text"
              value={formData.indexNumber}
              onChange={(e) => setFormData({ ...formData, indexNumber: e.target.value })}
              className="w-full px-4 py-2 border border-gray-300 rounded text-sm"
            />
          </div>
        )}
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
