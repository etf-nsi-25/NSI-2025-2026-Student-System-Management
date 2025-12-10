import React, { useState, useEffect, useCallback, useMemo } from "react";
import {
  CContainer,
  CRow,
  CCol,
  CCard,
  CCardBody,
  CTable,
  CTableHead,
  CTableRow,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
  CButton,
  CFormSelect,
  CFormInput,

} from "@coreui/react";

import CreateUserModal from "./CreateUserModal";
import EditUserModal from "./EditUserModal";
import DeactivateUserModal from "./DeactivateUserModal";
import UserDetailsModal from "./UserDetailsModal";

import { fetchUsers, getAvailableFaculties } from "../../service/identityApi";
import type { User, Role } from "../../service/identityApi";

import "../../styles/coreui-custom.css";

const ROLES: Role[] = ["Professor", "Assistant", "Student", "Staff"];
const availableFaculties = getAvailableFaculties();

const UserManagementPage: React.FC = () => {
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [isDeactivateModalOpen, setIsDeactivateModalOpen] = useState(false);

  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);

  const [selectedFaculty, setSelectedFaculty] = useState("");
  const [selectedRole, setSelectedRole] = useState("");
  const [searchText, setSearchText] = useState("");

  // Debug: log when create modal open state changes
  useEffect(() => {
    // eslint-disable-next-line no-console
    console.log('UserManagementPage: isCreateModalOpen =', isCreateModalOpen);
  }, [isCreateModalOpen]);

  const fetchUserList = useCallback(async () => {
    setLoading(true);
    try {
      const data = await fetchUsers();
      setUsers(data);
    } catch {
      setUsers([]);
    } finally {
      setLoading(false);
    }
  }, []);

  // When a new user is created, append it immediately for instant feedback,
  // then refresh the list from the mock API to keep authoritative state.
  const handleCreateSuccess = async (created?: User) => {
    if (created) {
      setUsers((prev) => [created, ...prev]);
    }
    // Ensure we refresh from source of truth
    await fetchUserList();
    setIsCreateModalOpen(false);
  };

  // When an existing user is updated, update it locally immediately and then refresh
  const handleUserUpdated = async (updated?: User) => {
    if (updated) {
      setUsers((prev) => prev.map((u) => (u.id === updated.id ? updated : u)));
    }
    await fetchUserList();
    setIsEditModalOpen(false);
  };

  // When a user is deactivated or deleted from the Deactivate modal, update local state
  const handleDeactivateSuccess = async (action: 'deactivated' | 'deleted') => {
    if (!selectedUser) {
      await fetchUserList();
      return;
    }

    if (action === 'deleted') {
      setUsers((prev) => prev.filter((u) => u.id !== selectedUser.id));
    } else if (action === 'deactivated') {
      setUsers((prev) => prev.map((u) => (u.id === selectedUser.id ? { ...u, status: 'Inactive' } : u)));
    }

    await fetchUserList();
  };

  useEffect(() => {
    fetchUserList();
  }, [fetchUserList]);

  const filteredUsers = useMemo(() => {
    const q = searchText.trim().toLowerCase();
    return users.filter((u) => {
      const fac = !selectedFaculty || u.facultyId === selectedFaculty;
      const role = !selectedRole || u.role === selectedRole;
      const text =
        !q ||
        u.firstName.toLowerCase().includes(q) ||
        u.lastName.toLowerCase().includes(q) ||
        u.email.toLowerCase().includes(q);

      return fac && role && text;
    });
  }, [users, selectedFaculty, selectedRole, searchText]);

  return (
    <div>
      <CContainer fluid className="d-flex justify-content-center">
        <CCol xs={12} className="d-flex flex-column align-items-center">


          {/* GORNJA BIJELA KARTICA */}
          <CCard className="filter-card">
            <CCardBody>

              {/* PRVI RED: Search | Faculty | Role */}
              <CRow className="mb-4">
                <CCol md={12} className="d-flex justify-content-center">
                  <div className="faculty-role-row">
                    <div className="faculty-col">
                      <label className="top-label" style={{ visibility: 'hidden' }}>Search</label>
                      {/* label intentionally removed for compact layout */}
                      <CFormInput
                        id="searchText"
                        name="searchText"
                        placeholder="Search for user.."
                        autoComplete="off"
                        value={searchText}
                        onChange={(e) => setSearchText(e.target.value)}
                        className="search-input"
                      />
                    </div>

                    <div className="faculty-col">
                      <label className="top-label" htmlFor="filter-faculty">Faculty</label>
                      <CFormSelect
                        id="filter-faculty"
                        name="filterFaculty"
                        value={selectedFaculty}
                        onChange={(e) => setSelectedFaculty(e.target.value)}
                        className="top-select"
                      >
                        <option value="">All</option>
                        {availableFaculties.map((f) => (
                          <option key={f.id} value={f.id}>{f.name}</option>
                        ))}
                      </CFormSelect>
                    </div>

                    <div className="faculty-col">
                      <label className="top-label" htmlFor="filter-role">Role</label>
                      <CFormSelect
                        id="filter-role"
                        name="filterRole"
                        value={selectedRole}
                        onChange={(e) => setSelectedRole(e.target.value)}
                        className="top-select"
                      >
                        <option value="">All</option>
                        {ROLES.map((r) => (
                          <option key={r}>{r}</option>
                        ))}
                      </CFormSelect>
                    </div>
                  </div>
                </CCol>
              </CRow>



              {/* TRECI RED: dugmad ispod inputa */}
              <div className="button-row">
                <CButton
                  className="btn-blue"
                  onClick={() => {
                    // eslint-disable-next-line no-console
                    console.log('+ Add User clicked');
                    setIsCreateModalOpen(true);
                  }}
                >
                  + Add User
                </CButton>

                <CButton className="btn-blue">+ Add in bulk</CButton>
              </div>

            </CCardBody>
          </CCard>

          {/* DONJA BIJELA KARTICA */}
          <CCard className="table-card">
            <CCardBody>

              <CTable hover responsive bordered className="user-table">
                <CTableHead color="light">
                  <CTableRow>
                    <CTableHeaderCell>Name</CTableHeaderCell>
                    <CTableHeaderCell>Email</CTableHeaderCell>
                    <CTableHeaderCell>Role</CTableHeaderCell>
                    <CTableHeaderCell>Last Active</CTableHeaderCell>
                    <CTableHeaderCell>Actions</CTableHeaderCell>
                  </CTableRow>
                </CTableHead>

                <CTableBody>
                  {filteredUsers.map((u) => (
                    <CTableRow key={u.id}>
                      <CTableDataCell>
                        {u.firstName} {u.lastName}
                      </CTableDataCell>

                      <CTableDataCell>{u.email}</CTableDataCell>
                      <CTableDataCell>{u.role}</CTableDataCell>
                      <CTableDataCell>{u.lastActive}</CTableDataCell>

                      <CTableDataCell>
                        <div className="action-btns">
                          <CButton
                            size="sm"
                            color="info"
                            variant="outline"
                            onClick={() => {
                              // debug
                              // eslint-disable-next-line no-console
                              console.log('UserManagementPage: Edit clicked', { id: u.id, username: u.username });
                              setSelectedUser(u);
                              setIsEditModalOpen(true);
                            }}
                          >
                            Edit
                          </CButton>

                          <CButton
                            size="sm"
                            color="danger"
                            variant="outline"
                            onClick={() => {
                              // debug
                              // eslint-disable-next-line no-console
                              console.log('UserManagementPage: Delete clicked', { id: u.id, username: u.username });
                              setSelectedUser(u);
                              setIsDeactivateModalOpen(true);
                            }}
                          >
                            Delete
                          </CButton>
                        </div>
                      </CTableDataCell>
                    </CTableRow>
                  ))}
                </CTableBody>
              </CTable>

              {/* Dugmad ispod tabele */}
              <div className="bottom-btn-row">
                <CButton className="btn-blue">Save attendance</CButton>
                <CButton variant="outline" color="secondary" className="btn-grey">
                  Export report
                </CButton>
              </div>
            </CCardBody>
          </CCard>

          {/* Modali */}
          <CreateUserModal isOpen={isCreateModalOpen} onClose={() => setIsCreateModalOpen(false)} availableFaculties={availableFaculties} onSuccess={handleCreateSuccess} />

          <UserDetailsModal isOpen={isDetailsModalOpen} onClose={() => setIsDetailsModalOpen(false)}
            user={selectedUser}
            onEditUser={(u) => {
              setSelectedUser(u);
              setIsDetailsModalOpen(false);
              setIsEditModalOpen(true);
            }}
          />

          <EditUserModal isOpen={isEditModalOpen} onClose={() => setIsEditModalOpen(false)} user={selectedUser} faculties={availableFaculties} onUpdated={handleUserUpdated} />

          <DeactivateUserModal isOpen={isDeactivateModalOpen} onClose={() => setIsDeactivateModalOpen(false)} user={selectedUser} onSuccess={handleDeactivateSuccess} />

        </CCol>
      </CContainer>
    </div>
  );
};

export default UserManagementPage;
