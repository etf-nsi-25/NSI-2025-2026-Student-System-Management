import React, { useState } from "react";
import { 
    CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter,
    CForm, CFormLabel, CFormInput, CFormSelect, CButton 
} from '@coreui/react';
import { createUser } from "../../service/identityApi"; 
import type { Faculty, Role, User  } from "../../service/identityApi"; 

interface Props {
    isOpen: boolean;
    onClose: () => void;
    availableFaculties: Faculty[];
    onSuccess: (created?: User) => void; 
}

const ROLES: Role[] = ['Professor', 'Assistant', 'Student', 'Staff'];

const initialFormState = {
    firstName: "", lastName: "", username: "", 
    password: "", role: "" as Role | "", faculty: "", indexNumber: ""
};

export default function CreateUserModal({ isOpen, onClose, availableFaculties, onSuccess }: Props) {
    const [form, setForm] = useState(initialFormState);
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<{ text: string; type: "success" | "danger" | "" }>({ text: "", type: "" });

    // (Debugging code removed) — modal no longer logs DOM diagnostics in production.

    // Keep behavior consistent with other modals: don't render modal markup when closed.
    if (!isOpen) return null;

    const validateForm = () => {
        const newErrors: { [key: string]: string } = {};

        if (!form.firstName) newErrors.firstName = "Ime je obavezno.";
        if (!form.lastName) newErrors.lastName = "Prezime je obavezno.";
        if (!form.username) newErrors.username = "Email je obavezan.";
        if (!form.password) newErrors.password = "Lozinka je obavezna.";
        if (!form.role) newErrors.role = "Rola je obavezna.";
        if (!form.faculty) newErrors.faculty = "Fakultet je obavezan.";
        
        if (form.role === "Student" && !form.indexNumber) {
            newErrors.indexNumber = "Broj indeksa je obavezan za studente.";
        }
        
       // validate that the username field contains a valid email (we use username as email)
       if (form.username && !/\S+@\S+\.\S+/.test(form.username)) {
           newErrors.username = "Email format nije validan.";
       }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };


    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        if (name === "role" && value !== "Student") {
            setForm({ ...form, role: value as Role, indexNumber: "" });
        } else {
            setForm({ ...form, [name]: value });
        }
        setErrors(prev => ({ ...prev, [name]: "" }));
        setMessage({ text: "", type: "" });
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) return;

        setLoading(true);
        setMessage({ text: "", type: "" });

        try {
            const dataToCreate = {
                firstName: form.firstName,
                lastName: form.lastName,
                email: form.username, // username field holds the user's email
                username: form.username,
                password: form.password,
                role: form.role as Role,
                facultyId: form.faculty,
                indexNumber: form.role === 'Student' ? form.indexNumber : undefined,
            };

            const created = await createUser(dataToCreate);

            setMessage({ text: "Korisnik uspješno kreiran!", type: "success" });
            
            setTimeout(() => {
                onSuccess(created); 
                setForm(initialFormState); 
                onClose(); 
            }, 1000);

        } catch (err: any) {
            const errorMsg = err?.message || "Greška pri kreiranju korisnika.";
            setMessage({ text: errorMsg, type: "danger" });
        } finally {
            setLoading(false);
        }
    };


    return (
        <CModal visible={isOpen} onClose={onClose} size="sm" className="modal-super-high-zindex" backdrop>
            <CModalHeader closeButton>
                <CModalTitle>Add User</CModalTitle>
            </CModalHeader>
            <CForm onSubmit={handleSave}>
                <CModalBody>
                    
                    <div className="mb-3">
                        <CFormLabel htmlFor="firstName">First Name</CFormLabel>
                        <CFormInput 
                            id="firstName"
                            name="firstName" 
                            autoComplete="given-name"
                            value={form.firstName} 
                            onChange={handleChange} 
                            invalid={!!errors.firstName} 
                        />
                        {errors.firstName && <div className="text-danger small">{errors.firstName}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="lastName">Last Name</CFormLabel>
                        <CFormInput 
                            id="lastName"
                            name="lastName" 
                            autoComplete="family-name"
                            value={form.lastName} 
                            onChange={handleChange} 
                            invalid={!!errors.lastName}
                        />
                         {errors.lastName && <div className="text-danger small">{errors.lastName}</div>}
                    </div>

                    {/* email input removed — username field will accept the user's email */}

                    <div className="mb-3">
                        <CFormLabel htmlFor="username">Username</CFormLabel>
                        <CFormInput 
                            id="username"
                            name="username" 
                            type="email"
                            autoComplete="email"
                            value={form.username} 
                            onChange={handleChange} 
                            invalid={!!errors.username}
                        />
                        {errors.username && <div className="text-danger small">{errors.username}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="password">Password</CFormLabel>
                        <CFormInput 
                            id="password"
                            type="password"
                            name="password" 
                            autoComplete="new-password"
                            value={form.password} 
                            onChange={handleChange} 
                            invalid={!!errors.password}
                        />
                         {errors.password && <div className="text-danger small">{errors.password}</div>}
                    </div>

                    <div className="mb-3">
                        <CFormLabel htmlFor="role">Select Role</CFormLabel>
                        <CFormSelect 
                            id="role"
                            name="role" 
                            value={form.role} 
                            onChange={handleChange}
                            invalid={!!errors.role}
                        >
                            <option value="">Select role</option>
                            {ROLES.map(r => <option key={r} value={r}>{r}</option>)}
                        </CFormSelect>
                        {errors.role && <div className="text-danger small">{errors.role}</div>}
                    </div>

                    {form.role === "Student" && (
                        <div className="mb-3">
                            <CFormLabel htmlFor="indexNumber">Index Number</CFormLabel>
                            <CFormInput
                                id="indexNumber"
                                name="indexNumber"
                                autoComplete="off"
                                value={form.indexNumber}
                                onChange={handleChange}
                                invalid={!!errors.indexNumber}
                            />
                            {errors.indexNumber && <div className="text-danger small">{errors.indexNumber}</div>}
                        </div>
                    )}

                    <div className="mb-3">
                        <CFormLabel htmlFor="faculty">Select Faculty</CFormLabel>
                        <CFormSelect 
                            id="faculty"
                            name="faculty" 
                            value={form.faculty} 
                            onChange={handleChange}
                            invalid={!!errors.faculty}
                        >
                            <option value="">Select Faculty</option>
                            {availableFaculties.map(f => (
                                <option key={f.id} value={f.id}>
                                    {f.name}
                                </option>
                            ))}
                        </CFormSelect>
                        {errors.faculty && <div className="text-danger small">{errors.faculty}</div>}
                    </div>

                    {/* Poruka o uspjehu/grešci */}
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
                        disabled={loading || message.type === 'success'}
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
}