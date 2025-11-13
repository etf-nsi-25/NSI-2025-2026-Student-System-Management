import React, { useState, useMemo } from 'react';
import type { ChangeEvent, FormEvent } from 'react';
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
    const [newFaculty, setNewFaculty] = useState<FacultyInput>({ name: '', abbreviation: '' });
    const [errors, setErrors] = useState<Partial<FacultyInput>>({});

    const filteredFaculties = useMemo(() => {
        return faculties.filter(f =>
            f.name.toLowerCase().includes(searchTerm.toLowerCase())
        );
    }, [faculties, searchTerm]);

    const sortedFaculties = useMemo(() => {
        return [...filteredFaculties].sort((a, b) => {
            if (a.name < b.name) return sortAsc ? -1 : 1;
            if (a.name > b.name) return sortAsc ? 1 : -1;
            return 0;
        });
    }, [filteredFaculties, sortAsc]);

    const totalPages = Math.ceil(sortedFaculties.length / itemsPerPage);
    const startIndex = (currentPage - 1) * itemsPerPage;
    const displayedFaculties = sortedFaculties.slice(startIndex, startIndex + itemsPerPage);

    const handleSortByName = () => setSortAsc(!sortAsc);

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setNewFaculty(prev => ({ ...prev, [name]: value }));
    };

    const validateForm = () => {
        const formErrors: Partial<FacultyInput> = {};
        if (!newFaculty.name.trim()) formErrors.name = 'Faculty name is required.';
        if (!newFaculty.abbreviation.trim()) formErrors.abbreviation = 'Abbreviation is required.';

        const exists = faculties.some(f =>
            f.name.toLowerCase() === newFaculty.name.toLowerCase() &&
            f.id !== editingFaculty?.id
        );
        if (exists) formErrors.name = 'Faculty name already exists.';

        setErrors(formErrors);
        return Object.keys(formErrors).length === 0;
    };

    const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (!validateForm()) return;

        if (editingFaculty) {
            // Edit existing
            setFaculties(prev =>
                prev.map(f => f.id === editingFaculty.id ? { ...f, ...newFaculty } : f)
            );
        } else {
            // Create new
            const newRecord: Faculty = {
                id: Date.now(),
                name: newFaculty.name,
                abbreviation: newFaculty.abbreviation,
                status: 'Active',
            };
            setFaculties(prev => [...prev, newRecord]);
        }

        setNewFaculty({ name: '', abbreviation: '' });
        setEditingFaculty(null);
        setErrors({});
        setShowModal(false);
    };

    const handleEdit = (faculty: Faculty) => {
        setEditingFaculty(faculty);
        setNewFaculty({ name: faculty.name, abbreviation: faculty.abbreviation });
        setShowModal(true);
    };

    const handleDelete = (faculty: Faculty) => {
        const confirm = window.confirm('Are you sure you want to delete this faculty?');
        if (!confirm) return;

        // Simulate API check for linked users/courses
        const linked = false; // replace with real check
        if (linked) {
            alert('Cannot delete faculty linked to users or courses.');
            return;
        }

        setFaculties(prev => prev.filter(f => f.id !== faculty.id));
    };

    return (
        <div className="faculty-listing-page">
            <h2>Faculty listing</h2>

            <div className="faculty-listing-controls">
                <input
                    type="text"
                    placeholder="Search by faculty name..."
                    value={searchTerm}
                    onChange={(e) => {
                        setSearchTerm(e.target.value);
                        setCurrentPage(1);
                    }}
                />
                <button onClick={() => { setEditingFaculty(null); setNewFaculty({ name: '', abbreviation: '' }); setShowModal(true); }}>
                    Add Faculty
                </button>
            </div>

            <table className="faculty-table">
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
                            <td className={faculty.status === 'Active' ? 'status-active' : 'status-inactive'}>
                                {faculty.status}
                            </td>
                            <td>
                                <button className="edit-btn" onClick={() => handleEdit(faculty)}>Edit</button>
                            </td>
                            <td>
                                <button className="delete-btn" onClick={() => handleDelete(faculty)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="pagination">
                <button
                    disabled={currentPage === 1}
                    onClick={() => setCurrentPage(currentPage - 1)}
                >
                    Prev
                </button>
                <span>Page {currentPage} of {totalPages}</span>
                <button
                    disabled={currentPage === totalPages}
                    onClick={() => setCurrentPage(currentPage + 1)}
                >
                    Next
                </button>
            </div>

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
                                {errors.abbreviation && <p className="error-text">{errors.abbreviation}</p>}
                            </div>

                            <div className="modal-actions">
                                <button type="button" onClick={() => setShowModal(false)}>Cancel</button>
                                <button type="submit">{editingFaculty ? 'Save Changes' : 'Create'}</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
