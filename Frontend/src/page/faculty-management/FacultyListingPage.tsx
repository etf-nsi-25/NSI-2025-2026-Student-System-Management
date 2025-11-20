import {
  useState,
  useMemo,
  type ChangeEvent,
  type FormEvent,
} from 'react';

import {
  CToaster,
  CToast,
  CToastHeader,
  CToastBody,
} from '@coreui/react';

import './FacultyListingPage.css';

type Faculty = {
  id: number;
  name: string;
  abbreviation: string;
  status: 'Active' | 'Inactive';
};

type FacultyInput = {
  name: string;
  abbreviation: string;
};

type ToastType = 'success' | 'error';

type ToastMessage = {
  id: number;
  type: ToastType;
  title: string;
  message: string;
};

export function FacultyListingPage() {
  const [faculties, setFaculties] = useState<Faculty[]>([
    { id: 1, name: 'Faculty of Engineering', abbreviation: 'ENG', status: 'Active' },
    { id: 2, name: 'Faculty of Medicine', abbreviation: 'MED', status: 'Active' },
    { id: 3, name: 'Faculty of Law', abbreviation: 'LAW', status: 'Inactive' },
    { id: 4, name: 'Faculty of Science', abbreviation: 'SCI', status: 'Active' },
    { id: 5, name: 'Faculty of Arts', abbreviation: 'ART', status: 'Inactive' },
  ]);

  const [searchTerm, setSearchTerm] = useState('');
  const [sortAsc, setSortAsc] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  const [showModal, setShowModal] = useState(false);
  const [editingFaculty, setEditingFaculty] = useState<Faculty | null>(null);
  const [newFaculty, setNewFaculty] = useState<FacultyInput>({
    name: '',
    abbreviation: '',
  });
  const [errors, setErrors] = useState<Partial<FacultyInput>>({});

  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const pushToast = (type: ToastType, title: string, message: string) => {
    setToasts((prev) => [
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
    setToasts((prev) => prev.filter((t) => t.id !== id));
  };

  /* FILTER + SORT */
  const filteredFaculties = useMemo(
    () =>
      faculties.filter((f) =>
        f.name.toLowerCase().includes(searchTerm.toLowerCase()),
      ),
    [faculties, searchTerm],
  );

  const sortedFaculties = useMemo(() => {
    return [...filteredFaculties].sort((a, b) => {
      if (a.name < b.name) return sortAsc ? -1 : 1;
      if (a.name > b.name) return sortAsc ? 1 : -1;
      return 0;
    });
  }, [filteredFaculties, sortAsc]);

  const totalPages = Math.ceil(sortedFaculties.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const displayedFaculties = sortedFaculties.slice(
    startIndex,
    startIndex + itemsPerPage,
  );

  const handleSortByName = () => setSortAsc((prev) => !prev);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setNewFaculty((prev) => ({ ...prev, [name]: value }));
  };

  const validateForm = () => {
    const formErrors: Partial<FacultyInput> = {};
    if (!newFaculty.name.trim())
      formErrors.name = 'Faculty name is required.';
    if (!newFaculty.abbreviation.trim())
      formErrors.abbreviation = 'Abbreviation is required.';

    const exists = faculties.some(
      (f) =>
        f.name.toLowerCase() === newFaculty.name.toLowerCase() &&
        f.id !== editingFaculty?.id,
    );
    if (exists) formErrors.name = 'Faculty name already exists.';

    setErrors(formErrors);

    if (Object.keys(formErrors).length > 0) {
      pushToast('error', 'Validation error', 'Please fix the highlighted fields.');
      return false;
    }

    return true;
  };

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!validateForm()) return;

    if (editingFaculty) {
      setFaculties((prev) =>
        prev.map((f) =>
          f.id === editingFaculty.id ? { ...f, ...newFaculty } : f,
        ),
      );
      pushToast('success', 'Faculty updated', 'Faculty has been updated successfully.');
    } else {
      const newRecord: Faculty = {
        id: Date.now(),
        name: newFaculty.name,
        abbreviation: newFaculty.abbreviation,
        status: 'Active',
      };
      setFaculties((prev) => [...prev, newRecord]);
      pushToast('success', 'Faculty created', 'Faculty has been created successfully.');
    }

    setNewFaculty({ name: '', abbreviation: '' });
    setEditingFaculty(null);
    setErrors({});
    setShowModal(false);
  };

  const handleEdit = (faculty: Faculty) => {
    setEditingFaculty(faculty);
    setNewFaculty({ name: faculty.name, abbreviation: faculty.abbreviation });
    setErrors({});
    setShowModal(true);
  };

  const handleDelete = (faculty: Faculty) => {
    const confirmDelete = window.confirm(
      'Are you sure you want to delete this faculty?',
    );
    if (!confirmDelete) return;

    const linked = false;

    if (linked) {
      pushToast(
        'error',
        'Delete blocked',
        'Faculty is linked to other entities and cannot be deleted.',
      );
      return;
    }

    setFaculties((prev) => prev.filter((f) => f.id !== faculty.id));
    pushToast('success', 'Faculty deleted', 'Faculty has been deleted successfully.');
  };

  const openCreateModal = () => {
    setEditingFaculty(null);
    setNewFaculty({ name: '', abbreviation: '' });
    setErrors({});
    setShowModal(true);
  };

  return (
    <>
      <div className="fl-card">
        <div className="fl-header">
          <h2>Faculty Management</h2>
        </div>

        {/* Controls */}
        <div className="fl-controls">
          <input
            type="text"
            placeholder="Search by faculty name..."
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setCurrentPage(1);
            }}
          />
          <button type="button" onClick={openCreateModal}>
            Add Faculty
          </button>
        </div>

        {/* Table */}
        <div className="fl-table-wrapper">
          <table className="fl-table">
            <thead>
              <tr>
                <th onClick={handleSortByName} className="sortable">
                  Faculty Name {sortAsc ? '▲' : '▼'}
                </th>
                <th>Abbreviation</th>
                <th>Status</th>
                <th>Edit</th>
                <th>Delete</th>
              </tr>
            </thead>

            <tbody>
              {displayedFaculties.map((faculty) => (
                <tr key={faculty.id}>
                  <td>{faculty.name}</td>
                  <td>{faculty.abbreviation}</td>
                  <td
                    className={
                      faculty.status === 'Active'
                        ? 'status-active'
                        : 'status-inactive'
                    }
                  >
                    {faculty.status}
                  </td>
                  <td>
                    <button
                      type="button"
                      className="edit-btn"
                      onClick={() => handleEdit(faculty)}
                    >
                      Edit
                    </button>
                  </td>
                  <td>
                    <button
                      type="button"
                      className="delete-btn"
                      onClick={() => handleDelete(faculty)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        <div className="pagination">
          <button
            disabled={currentPage === 1}
            onClick={() => setCurrentPage((p) => p - 1)}
          >
            Prev
          </button>
          <span>
            Page {currentPage} of {totalPages || 1}
          </span>
          <button
            disabled={currentPage === totalPages || totalPages === 0}
            onClick={() => setCurrentPage((p) => p + 1)}
          >
            Next
          </button>
        </div>

        {/* Modal */}
        {showModal && (
          <div className="modal-backdrop">
            <div className="modal">
              <h3>{editingFaculty ? 'Edit Faculty' : 'Create Faculty'}</h3>

              <form onSubmit={handleSubmit}>
                <div className="form-group">
                  <label>Faculty Name:</label>
                  <input
                    type="text"
                    name="name"
                    value={newFaculty.name}
                    onChange={handleChange}
                  />
                  {errors.name && <p className="error-text">{errors.name}</p>}
                </div>

                <div className="form-group">
                  <label>Abbreviation:</label>
                  <input
                    type="text"
                    name="abbreviation"
                    value={newFaculty.abbreviation}
                    onChange={handleChange}
                  />
                  {errors.abbreviation && (
                    <p className="error-text">{errors.abbreviation}</p>
                  )}
                </div>

                <div className="modal-actions">
                  <button type="button" onClick={() => setShowModal(false)}>
                    Cancel
                  </button>
                  <button type="submit">
                    {editingFaculty ? 'Save Changes' : 'Create'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>

      {/* Toasts */}
      <CToaster placement="top-end">
        {toasts.map((toast) => (
          <CToast
            key={toast.id}
            visible
            color={toast.type === 'success' ? 'success' : 'danger'}
            onClose={() => removeToast(toast.id)}
          >
            <CToastHeader closeButton>
              {toast.title}
            </CToastHeader>
            <CToastBody>{toast.message}</CToastBody>
          </CToast>
        ))}
      </CToaster>
    </>
  );
}
