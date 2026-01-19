import React, { useState, useEffect } from "react";
import {
    CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter,
    CForm, CFormLabel, CFormInput, CFormSelect, CButton
} from '@coreui/react';
import { useAPI } from "../../context/services";
import { updateUser } from "../../service/identityApi";
import type { User, Faculty, Role, Status } from "../../service/identityApi";
import { useAuthContext } from "../../init/auth";

interface EditUserModalProps {
    isOpen: boolean;
    onClose: () => void;
    user: User | null;
    faculties: Faculty[];
    onUpdated: (updated?: User) => void;
}

const EditUserModal: React.FC<EditUserModalProps> = ({
    isOpen, onClose, user, faculties, onUpdated,
}) => {
    const api = useAPI();
    const { authInfo } = useAuthContext();
    const [form, setForm] = useState({
        firstName: "", lastName: "", facultyId: "", role: "", indexNumber: "", status: "",
    });

    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [message, setMessage] = useState<{ text: string; type: "success" | "danger" | "" }>({
        text: "", type: "",
    });

    const availableRoles = ['Professor', 'Assistant', 'Student', 'Admin', 'Superadmin'].filter(r => {
        // Must allow keeping current role even if generally restricted (e.g. looking at an Admin)
        // But for *changing* role, restrictions apply.
        // Simplified Logic per request:
        if (authInfo?.role === 'Superadmin') {
            // Superadmin can only manage Admins. 
            // Exception: If they are editing an existing Admin, 'Admin' is valid.
            return r === 'Admin';
        }
        if (authInfo?.role === 'Admin') return r !== 'Superadmin' && r !== 'Admin';
        return r !== 'Superadmin' && r !== 'Admin';
    });

    // Učitavanje trenutnih korisničkih podataka u formu
    useEffect(() => {
        if (user) {
            setForm({
                firstName: user.firstName,
                lastName: user.lastName,
                facultyId: user.facultyId,
                role: user.role,
                indexNumber: user.indexNumber || "",
                status: user.status || "Active",
            });
            setMessage({ text: "", type: "" });
            setErrors({});
        }
    }, [user]);

    if (!isOpen || !user) return null;

    const validateForm = () => {
        const newErrors: { [key: string]: string } = {};

        if (!form.firstName) newErrors.firstName = "Ime je obavezno.";
        if (!form.lastName) newErrors.lastName = "Prezime je obavezno.";
        if (!form.role) newErrors.role = "Rola je obavezna.";
        if (!form.facultyId) newErrors.facultyId = "Fakultet je obavezan.";

        if (form.role === "Student" && !form.indexNumber) {
            newErrors.indexNumber = "Broj indeksa je obavezan za studente.";
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        if (name === "role" && value !== "Student") {
            setForm({ ...form, role: value, indexNumber: "" });
        } else {
            setForm({ ...form, [name]: value });
        }
        setErrors(prev => ({ ...prev, [name]: "" }));
        setMessage({ text: "", type: "" });
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) return;

        try {
            setLoading(true);

            const payload = {
                firstName: form.firstName,
                lastName: form.lastName,
                facultyId: form.facultyId,
                role: form.role as Role,
                indexNumber: form.role === "Student" ? form.indexNumber : "",
                status: form.status as Status,
                // Include required fields that are not editable but required by DTO
                email: user.email,
                username: user.username
            };
            // debug
            // eslint-disable-next-line no-console
            console.log('EditUserModal: updating user', { id: user.id, payload });

            const updated = await updateUser(api, user.id, payload as any); // <--- UPDATED
            // eslint-disable-next-line no-console
            console.log('EditUserModal: updateUser returned', updated);

            setMessage({ text: "Korisnik uspješno ažuriran!", type: "success" });

            setTimeout(() => {
                // pass updated user back to parent so it can update UI immediately
                onUpdated(updated);
                onClose();
            }, 800);
        } catch (err: any) {
            setMessage({ text: err?.message || "Ažuriranje nije uspjelo. Pokušajte ponovo.", type: "danger" });
        } finally {
            setLoading(false);
        }
    };


    return (
        <CModal visible={isOpen} onClose={onClose} size="sm" className="modal-super-high-zindex">
            <CModalHeader closeButton>
                <CModalTitle>Edit User</CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSave}>
                <CModalBody>

                    <div className="mb-3">
                        <CFormLabel htmlFor="edit-firstName">First Name</CFormLabel>
                        <CFormInput
                            id="edit-firstName"
                            name="firstName"
                            autoComplete="given-name"
                            value={form.firstName}
                            onChange={handleChange}
                            invalid={!!errors.firstName}
                        />
                        {errors.firstName && <div className="text-danger small">{errors.firstName}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="edit-lastName">Last Name</CFormLabel>
                        <CFormInput
                            id="edit-lastName"
                            name="lastName"
                            autoComplete="family-name"
                            value={form.lastName}
                            onChange={handleChange}
                            invalid={!!errors.lastName}
                        />
                        {errors.lastName && <div className="text-danger small">{errors.lastName}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="edit-facultyId">Faculty</CFormLabel>
                        <CFormSelect
                            id="edit-facultyId"
                            name="facultyId"
                            value={authInfo?.role === 'Admin' ? authInfo.tenantId : form.facultyId}
                            onChange={handleChange}
                            invalid={!!errors.facultyId}
                            disabled={authInfo?.role === 'Admin'}
                        >
                            {faculties.map((f) => (
                                <option key={f.id} value={f.id}>
                                    {f.name}
                                </option>
                            ))}
                        </CFormSelect>
                        {errors.facultyId && <div className="text-danger small">{errors.facultyId}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="edit-role">Role</CFormLabel>
                        <CFormSelect
                            id="edit-role"
                            name="role"
                            value={form.role}
                            onChange={handleChange}
                            invalid={!!errors.role}
                        >
                            {availableRoles.map(r => (
                                <option key={r} value={r}>{r}</option>
                            ))}
                        </CFormSelect>
                        {errors.role && <div className="text-danger small">{errors.role}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="edit-status">Status</CFormLabel>
                        <CFormSelect
                            id="edit-status"
                            name="status"
                            value={form.status}
                            onChange={handleChange}
                        >
                            <option value="Active">Active</option>
                            <option value="Inactive">Inactive</option>
                        </CFormSelect>
                    </div>

                    {form.role === "Student" && (
                        <div className="mb-3">
                            <CFormLabel htmlFor="edit-indexNumber">Index Number</CFormLabel>
                            <CFormInput
                                id="edit-indexNumber"
                                name="indexNumber"
                                autoComplete="off"
                                value={form.indexNumber}
                                onChange={handleChange}
                                invalid={!!errors.indexNumber}
                            />
                            {errors.indexNumber && <div className="text-danger small">{errors.indexNumber}</div>}
                        </div>
                    )}


                    {message.text && (
                        <div className={`p-2 mt-3 text-center alert alert-${message.type}`}>
                            {message.text}
                        </div>
                    )}
                </CModalBody>
                <CModalFooter>
                    <CButton
                        color="primary"
                        type="submit"
                        disabled={loading}
                    >
                        {loading ? "Saving..." : "Save"}
                    </CButton>
                    <CButton color="secondary" onClick={onClose} disabled={loading}>
                        Cancel
                    </CButton>
                </CModalFooter>
            </CForm>
        </CModal>
    );
};

export default EditUserModal;