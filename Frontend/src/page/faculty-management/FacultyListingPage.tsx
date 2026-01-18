import { useState, useMemo, useEffect, type ChangeEvent, type FormEvent } from 'react';
import {
  CModal,
  CModalHeader,
  CModalTitle,
  CModalBody,
  CModalFooter,
  CButton,
  CForm,
  CFormInput,
} from '@coreui/react';

import './FacultyListingPage.css';
import { useAPI } from '../../context/services';
import type { FacultyResponseDTO } from '../../dto/FacultyDTO';

type FacultyInput = {
  name: string;
  address: string;
  code: string;
};

type ToastType = 'success' | 'error';

type ToastMessage = {
  id: number;
  type: ToastType;
  title: string;
  message: string;
};

// TODO: This entire page is a travesty and should be completely refactored.

export function FacultyListingPage() {
  const api = useAPI();
  const [faculties, setFaculties] = useState<FacultyResponseDTO[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortAsc, setSortAsc] = useState(true);

  const [showModal, setShowModal] = useState(false);
  const [editingFaculty, setEditingFaculty] = useState<FacultyResponseDTO | null>(null);
  const [newFaculty, setNewFaculty] = useState<FacultyInput>({
    name: '',
    address: '',
    code: '',
  });
  const [errors, setErrors] = useState<Partial<FacultyInput>>({});

  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const pushToast = (type: ToastType, title: string, message: string) => {
    setToasts(prev => [
      ...prev,
      {
        id: Date.now() + Math.random(),
        type,
        title,
        message,
      },
    ]);
  };

  const removeToast = (id: number) => {
    setToasts(prev => prev.filter(t => t.id !== id));
  };

  const fetchFaculties = async () => {
    try {
      const data = await api.getFaculties();
      setFaculties(data);
    } catch (e: any) {
      pushToast('error', 'Load failed', e.message ?? 'Failed to load faculties');
    }
  };

  const createFaculty = async (input: FacultyInput): Promise<FacultyResponseDTO | null> => {
    try {
      const dto = await api.createFaculty(input);
      pushToast('success', 'Faculty created', 'Faculty has been created successfully.');
      return dto;
    } catch (e: any) {
      pushToast('error', 'Create failed', e.message);
      return null;
    }
  };

  const updateFaculty = async (id: string, input: FacultyInput): Promise<FacultyResponseDTO | null> => {
    try {
      const updatedDto = await api.updateFaculty(id, input);
      pushToast('success', 'Faculty updated', 'Faculty has been updated successfully.');
      return updatedDto;
    } catch (e: any) {
      pushToast('error', 'Update failed', e.message);
      return null;
    }
  };

  const deleteFaculty = async (id: string): Promise<boolean> => {
    try {
      await api.deleteFaculty(id);
      pushToast('success', 'Faculty deleted', 'Faculty has been deleted successfully.');
      return true;
    } catch (e: any) {
      pushToast('error', 'Delete failed', e.message);
      return false;
    }
  };

  useEffect(() => {
    fetchFaculties();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  /* FILTER + SORT */
  const filteredFaculties = useMemo(
    () => faculties.filter(f => f.name.toLowerCase().includes(searchTerm.toLowerCase())),
    [faculties, searchTerm]
  );

  const sortedFaculties = useMemo(() => {
    return [...filteredFaculties].sort((a, b) => {
      if (a.name < b.name) return sortAsc ? -1 : 1;
      if (a.name > b.name) return sortAsc ? 1 : -1;
      return 0;
    });
  }, [filteredFaculties, sortAsc]);

  const handleSortByName = () => setSortAsc(prev => !prev);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setNewFaculty(prev => ({ ...prev, [name]: value }));
  };

  const validateForm = () => {
    const formErrors: Partial<FacultyInput> = {};
    if (!newFaculty.name.trim()) formErrors.name = 'Faculty name is required.';
    if (!newFaculty.address.trim()) formErrors.address = 'Address is required.';
    if (!newFaculty.code.trim()) formErrors.code = 'Code is required.';

    const exists = faculties.some(
      f => f.name.toLowerCase() === newFaculty.name.toLowerCase() && f.id !== editingFaculty?.id
    );
    if (exists) formErrors.name = 'Faculty name already exists.';

    setErrors(formErrors);

    if (Object.keys(formErrors).length > 0) {
      pushToast('error', 'Validation error', 'Please fix the highlighted fields.');
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!validateForm()) return;

    if (editingFaculty) {
      // UPDATE
      const updated = await updateFaculty(editingFaculty.id, newFaculty);
      if (updated) {
        setFaculties(prev => prev.map(f => (f.id === updated.id ? updated : f)));
      }
    } else {
      // CREATE
      const created = await createFaculty(newFaculty);
      if (created) {
        setFaculties(prev => [...prev, created]);
      }
    }

    setNewFaculty({ name: '', address: '', code: '' });
    setEditingFaculty(null);
    setErrors({});
    setShowModal(false);
  };

  const handleEdit = (faculty: FacultyResponseDTO) => {
    setEditingFaculty(faculty);
    setNewFaculty({
      name: faculty.name,
      address: faculty.address,
      code: faculty.code,
    });
    setErrors({});
    setShowModal(true);
  };

  const handleDelete = async (faculty: FacultyResponseDTO) => {
    const confirmDelete = window.confirm('Are you sure you want to delete this faculty?');
    if (!confirmDelete) return;

    const ok = await deleteFaculty(faculty.id);
    if (!ok) return;

    setFaculties(prev => prev.filter(f => f.id !== faculty.id));
  };

  const openCreateModal = () => {
    setEditingFaculty(null);
    setNewFaculty({ name: '', address: '', code: '' });
    setErrors({});
    setShowModal(true);
  };

  return (
    <>
      <div className='fl-card'>
        <div className='fl-header'>
          <h2>Faculty Management</h2>
        </div>

        {/* Controls */}
        <div className='fl-controls'>
          <input
            type='text'
            placeholder='Search by faculty name...'
            value={searchTerm}
            onChange={e => {
              setSearchTerm(e.target.value);
            }}
          />
          <button type='button' onClick={openCreateModal}>
            Add Faculty
          </button>
        </div>

        {/* Table */}
        <div className='fl-table-wrapper'>
          <table className='fl-table'>
            <thead>
              <tr>
                <th onClick={handleSortByName} className='sortable'>
                  Faculty Name {sortAsc ? '▲' : '▼'}
                </th>
                <th>Address</th>
                <th>Code</th>
                <th>Edit</th>
                <th>Delete</th>
              </tr>
            </thead>

            <tbody>
              {sortedFaculties.map(faculty => (
                <tr key={faculty.id}>
                  <td>{faculty.name}</td>
                  <td>{faculty.address}</td>
                  <td>{faculty.code}</td>
                  <td>
                    <button type='button' className='edit-btn' onClick={() => handleEdit(faculty)}>
                      Edit
                    </button>
                  </td>
                  <td>
                    <button type='button' className='delete-btn' onClick={() => handleDelete(faculty)}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <CModal
          visible={showModal}
          onClose={() => setShowModal(false)}
          alignment="center"
          size="lg"
          className="modal-z-fix"
        >
          <CModalHeader>
            <CModalTitle>{editingFaculty ? 'Edit Faculty' : 'Create Faculty'}</CModalTitle>
          </CModalHeader>
          <CModalBody>
            <CForm onSubmit={handleSubmit}>
              <div className='mb-3'>
                <CFormInput
                  label="Faculty Name"
                  type='text'
                  name='name'
                  value={newFaculty.name ?? ''}
                  onChange={handleChange}
                  feedback={errors.name}
                  invalid={!!errors.name}
                />
              </div>

              <div className='mb-3'>
                <CFormInput
                  label="Address"
                  type='text'
                  name='address'
                  value={newFaculty.address ?? ''}
                  onChange={handleChange}
                  feedback={errors.address}
                  invalid={!!errors.address}
                />
              </div>

              <div className='mb-3'>
                <CFormInput
                  label="Code"
                  type='text'
                  name='code'
                  value={newFaculty.code ?? ''}
                  onChange={handleChange}
                  feedback={errors.code}
                  invalid={!!errors.code}
                />
              </div>
            </CForm>
          </CModalBody>
          <CModalFooter>
            <CButton color="secondary" onClick={() => setShowModal(false)}>
              Cancel
            </CButton>
            <CButton color="primary" onClick={() => {
              const fakeEvent = { preventDefault: () => { } } as any;
              handleSubmit(fakeEvent);
            }}>
              {editingFaculty ? 'Save Changes' : 'Create'}
            </CButton>
          </CModalFooter>
        </CModal>
      </div>

      {/* Toasts – custom, no CoreUI */}
      <div className='fl-toast-container'>
        {toasts.map(toast => (
          <div key={toast.id} className={`fl-toast fl-toast-${toast.type}`}>
            <div className='fl-toast-header'>
              <strong>{toast.title}</strong>
              <button type='button' className='fl-toast-close' onClick={() => removeToast(toast.id)}>
                ×
              </button>
            </div>
            <div className='fl-toast-body'>{toast.message}</div>
          </div>
        ))}
      </div>
    </>
  );
}
