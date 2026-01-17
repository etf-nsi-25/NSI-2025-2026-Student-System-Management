import { useState, useMemo, useEffect, type ChangeEvent, type FormEvent } from 'react';

import './FacultyListingPage.css';
import { useAPI } from '../../context/services';
import type { FacultyResponseDTO } from '../../dto/FacultyDTO';

type Faculty = {
  id: number;
  name: string;
  address: string;
  code: string;
};

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

function mapFaculty(dto: FacultyResponseDTO): Faculty {
  return {
    id: dto.id,
    name: dto.name,
    address: dto.address,
    code: dto.code,
  };
}

// TODO: This entire page is a travesty and should be completely refactored.

export function FacultyListingPage() {
  const api = useAPI();
  const [faculties, setFaculties] = useState<Faculty[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortAsc, setSortAsc] = useState(true);

  const [showModal, setShowModal] = useState(false);
  const [editingFaculty, setEditingFaculty] = useState<Faculty | null>(null);
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
      setFaculties(data.map(mapFaculty));
    } catch (e: any) {
      pushToast('error', 'Load failed', e.message ?? 'Failed to load faculties');
    }
  };

  const createFaculty = async (input: FacultyInput): Promise<Faculty | null> => {
    try {
      const dto = await api.createFaculty(input);
      pushToast('success', 'Faculty created', 'Faculty has been created successfully.');
      return mapFaculty(dto);
    } catch (e: any) {
      pushToast('error', 'Create failed', e.message);
      return null;
    }
  };

  const updateFaculty = async (id: number, input: FacultyInput): Promise<Faculty | null> => {
    try {
      const updatedDto = await api.updateFaculty(id, input);
      pushToast('success', 'Faculty updated', 'Faculty has been updated successfully.');
      return mapFaculty(updatedDto);
    } catch (e: any) {
      pushToast('error', 'Update failed', e.message);
      return null;
    }
  };

  const deleteFaculty = async (id: number): Promise<boolean> => {
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

  const handleEdit = (faculty: Faculty) => {
    setEditingFaculty(faculty);
    setNewFaculty({
      name: faculty.name,
      address: faculty.address,
      code: faculty.code,
    });
    setErrors({});
    setShowModal(true);
  };

  const handleDelete = async (faculty: Faculty) => {
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

        {/* Modal */}
        {showModal && (
          <div className='modal-backdrop'>
            <div className='modal'>
              <h3>{editingFaculty ? 'Edit Faculty' : 'Create Faculty'}</h3>

              <form onSubmit={handleSubmit}>
                <div className='form-group'>
                  <label>Faculty Name:</label>
                  <input type='text' name='name' value={newFaculty.name ?? ''} onChange={handleChange} />
                  {errors.name && <p className='error-text'>{errors.name}</p>}
                </div>

                <div className='form-group'>
                  <label>Address:</label>
                  <input type='text' name='address' value={newFaculty.address ?? ''} onChange={handleChange} />
                  {errors.address && <p className='error-text'>{errors.address}</p>}
                </div>

                <div className='form-group'>
                  <label>Code:</label>
                  <input type='text' name='code' value={newFaculty.code ?? ''} onChange={handleChange} />
                  {errors.code && <p className='error-text'>{errors.code}</p>}
                </div>

                <div className='modal-actions'>
                  <button type='button' onClick={() => setShowModal(false)}>
                    Cancel
                  </button>
                  <button type='submit'>{editingFaculty ? 'Save Changes' : 'Create'}</button>
                </div>
              </form>
            </div>
          </div>
        )}
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
