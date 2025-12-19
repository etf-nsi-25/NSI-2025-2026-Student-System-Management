import React, { useState } from "react";
import type { User } from "../../../types/user-types";
import Modal from "../../shared/Modal";
import { CForm, CFormInput, CFormSelect, CButton, CFormLabel } from '@coreui/react';
import { ROLE_MAP } from "../../../page/userManagement";



interface EditUserModalProps {
  user: User;
  onClose: () => void;
  onSave: (user: User) => void;
}

export default function EditUserModal({ user, onClose, onSave }: EditUserModalProps) {
  const findRoleKey = (label: string | number | undefined) => {
    const entry = Object.entries(ROLE_MAP).find(([, v]) => v === label || v === String(label));
    return entry ? entry[0] : '5';
  };

  const [formData, setFormData] = useState(() => ({
    ...user,
    role: String(findRoleKey(user.role as any)),
  } as any));

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <Modal onClose={onClose}>
      <h2 className="h5 text-primary mb-3">Edit User</h2>

      <CForm onSubmit={handleSubmit}>
        <div className="mb-2">
          <CFormLabel htmlFor="firstName" className="text-dark">First Name</CFormLabel>
          <CFormInput
            id="firstName"
            placeholder="Enter first name"
            value={formData.firstName}
            onChange={(e: any) => setFormData({ ...formData, firstName: e.target.value })}
            required
          />
        </div>

        <div className="mb-2">
          <CFormLabel htmlFor="lastName" className="text-dark">Last Name</CFormLabel>
          <CFormInput
            id="lastName"
            placeholder="Enter last name"
            value={formData.lastName}
            onChange={(e: any) => setFormData({ ...formData, lastName: e.target.value })}
            required
          />
        </div>

        <div className="mb-2">
          <CFormLabel htmlFor="roleSelect" className="text-dark">Role</CFormLabel>
          <CFormSelect
            id="roleSelect"
            value={formData.role}
            onChange={(e: any) => setFormData({ ...formData, role: e.target.value })}
          >
            {Object.entries(ROLE_MAP).map(([k, v]) => (
              <option key={k} value={k}>{v}</option>
            ))}
          </CFormSelect>
        </div>

        <div className="mb-2">
          <CFormLabel htmlFor="facultySelect" className="text-dark">Faculty</CFormLabel>
          <CFormSelect
            id="facultySelect"
            value={formData.faculty}
            onChange={(e: any) => setFormData({ ...formData, faculty: e.target.value })}
          >
            <option>ETF UNSA</option>
          </CFormSelect>
        </div>

        {Number(formData.role) === 5 && (
          <div className="mb-2">
            <CFormLabel htmlFor="indexNumber" className="text-dark">Index Number</CFormLabel>
            <CFormInput
              id="indexNumber"
              placeholder="Enter index number"
              value={formData.indexNumber}
              onChange={(e: any) => setFormData({ ...formData, indexNumber: e.target.value })}
            />
          </div>


        )}
        <div className="d-flex justify-content-end gap-2 mt-3">
          <CButton color="secondary" onClick={onClose} type="button">Cancel</CButton>
          <CButton color="primary" type="submit">Save</CButton>
        </div>
      </CForm>






    </Modal>
  );
}
