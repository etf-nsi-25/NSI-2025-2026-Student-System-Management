import React, { useState } from "react";
import {
    CModal, CModalHeader, CModalTitle, CModalBody, CModalFooter,
    CButton
} from '@coreui/react';
import { useAPI } from "../../context/services"; // <--- ADDED
import { deactivateUser, deleteUser } from "../../service/identityApi";
import type { User } from "../../service/identityApi"; // <--- ISPRAVLJENO

interface Props {
    isOpen: boolean;
    onClose: () => void;
    user: User | null;
    onSuccess: (action: 'deactivated' | 'deleted') => void;
}

const DeactivateUserModal: React.FC<Props> = ({ isOpen, onClose, user, onSuccess }) => {
    const api = useAPI(); // <--- ADDED
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<{ text: string; type: "success" | "danger" | "" }>({ text: "", type: "" });

    if (!isOpen || !user) return null;

    const handleDeactivate = async () => {
        setLoading(true);
        setMessage({ text: "", type: "" });
        try {
            // debug
            // eslint-disable-next-line no-console
            console.log('DeactivateUserModal: deactivating user', { id: user.id, username: user.username });

            const result = await deactivateUser(api, user.id); // <--- UPDATED
            // eslint-disable-next-line no-console
            console.log('DeactivateUserModal: deactivateUser returned', result);
            setMessage({ text: "Korisnik uspješno deaktiviran.", type: "success" });
            setTimeout(() => {
                onSuccess('deactivated');
                onClose();
            }, 800);
        } catch (err: any) {
            setMessage({ text: err?.message || "Deaktivacija nije uspjela.", type: "danger" });
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async () => {
        if (!confirm(`Da li ste sigurni da želite TRAJNO izbrisati korisnika ${user.username}? Ova akcija se ne može povući.`)) {
            return;
        }

        setLoading(true);
        setMessage({ text: "", type: "" });
        try {
            // debug
            // eslint-disable-next-line no-console
            console.log('DeactivateUserModal: deleting user', { id: user.id, username: user.username });

            await deleteUser(api, user.id); // <--- UPDATED
            // eslint-disable-next-line no-console
            console.log('DeactivateUserModal: deleteUser completed for', user.id);
            setMessage({ text: "Korisnik uspješno izbrisan.", type: "success" });
            setTimeout(() => {
                onSuccess('deleted');
                onClose();
            }, 800);
        } catch (err: any) {
            setMessage({ text: err?.message || "Brisanje nije uspjelo.", type: "danger" });
        } finally {
            setLoading(false);
        }
    };

    return (
        <CModal visible={isOpen} onClose={onClose} alignment="center" className="modal-super-high-zindex">
            <CModalHeader closeButton>
                <CModalTitle>Akcije za Korisnika</CModalTitle>
            </CModalHeader>
            <CModalBody className="text-center">

                <p className="h5 mb-3">**{user.firstName} {user.lastName}** ({user.role})</p>
                <p className="text-danger mb-4">Odaberite željenu akciju za ovog korisnika.</p>

                {message.text && (
                    <div className={`p-2 mb-3 alert alert-${message.type}`}>
                        {message.text}
                    </div>
                )}
            </CModalBody>
            <CModalFooter className="justify-content-center flex-column gap-2">

                <CButton
                    color="warning"
                    onClick={handleDeactivate}
                    disabled={loading || user.status === 'Inactive'}
                    className="w-100"
                >
                    {loading && message.type !== 'success' ? "Deactivating..." : (user.status === 'Inactive' ? "Već Neaktivan" : "Deaktiviraj")}
                </CButton>

                <CButton
                    color="danger"
                    onClick={handleDelete}
                    disabled={loading}
                    className="w-100"
                >
                    {loading && message.type !== 'success' ? "Deleting..." : "Obriši Trajno"}
                </CButton>

                <CButton color="secondary" onClick={onClose} disabled={loading} className="w-100 mt-2">
                    Poništi
                </CButton>
            </CModalFooter>
        </CModal>
    );
};

export default DeactivateUserModal;