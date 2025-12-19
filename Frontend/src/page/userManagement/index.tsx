import { useState, useEffect } from 'react';
import SearchFilters from '../../component/user-management/SearchFilters';
import UsersTable from '../../component/user-management/UsersTable';
import UserDetailsPanel from '../../component/user-management/UserDetailsPanel';
import AddUserModal from '../../component/user-management/modals/AddUserModal';
import EditUserModal from '../../component/user-management/modals/EditUserModal';
import DeleteConfirmDialog from '../../component/user-management/modals/DeleteUserDialog';
import Header from '../../component/shared/Header';
import { CContainer, CRow, CCol } from '@coreui/react';
import type { User } from '../../types/user-types';
import { useAPI } from '../../context/services';
import ToastManager from '../../component/Toast';


export const ROLE_MAP: Record<number, string> = {
  1: "Superadmin",
  2: "Admin",
  3: "Teacher",
  4: "Assistant",
  5: "Student"
};


export default function UserManagement() {
  const [users, setUsers] = useState<User[]>([
  ]);

  const api = useAPI();


  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [faculty, setFaculty] = useState('ETF UNSA');
  const [role, setRole] = useState('All');
  const [searchTerm, setSearchTerm] = useState('');

  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error'; visible: boolean } | null>(null);

  const showToast = (message: string, type: 'success' | 'error') => {
    setToast({ message, type, visible: true });
    setTimeout(() => setToast(prev => prev ? { ...prev, visible: false } : null), 3000);
  };
  const filteredUsers = users.filter(u => {
    const matchesFaculty = faculty === 'All' || u?.faculty === faculty;
    const matchesRole = role === 'All' || u.role === ROLE_MAP[Number(role)];
    const matchesSearch =
      (u?.username ?? '').toLowerCase().includes(searchTerm.toLowerCase()) || (u.role ?? '').toLowerCase().includes(searchTerm.toLowerCase());
    return matchesFaculty && matchesRole && matchesSearch;
  });

  const getAllUsers = async () => {
    try {
      const response: any = await api.get('/api/users');
      if (!response || !response.items) {
        throw new Error('Invalid response format');
      }

      const parsedUsers = (response.items || []).filter(Boolean).map((user: any) => ({
        id: user.id,
        firstName: user.firstName ?? '',
        lastName: user.lastName ?? '',
        username: user.username ?? '',
        role: ROLE_MAP[user.role] ?? 'Unknown',
        faculty: user.faculty ?? 'ETF UNSA',
        lastActive: user.lastActive ?? 'Unknown',
        indexNumber: user.indexNumber ?? null,
      }));
      setUsers(parsedUsers);

    } catch (error) {
      showToast('Failed to fetch users', 'error');
    }
  };

  useEffect(() => {
    getAllUsers();
  }, []);

  const handleAddUser = async (newUser: any) => {
    const requestBody = {

      username: newUser.username,
      password: newUser.password,
      firstName: newUser.firstName,
      lastName: newUser.lastName,
      email: `${newUser.username}@unsa.ba`,
      facultyId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
      indexNumber: newUser.indexNumber || null,
      role: Number(newUser.role ?? 5),

    };

    try {
      await api.post('/api/users', requestBody);
      await getAllUsers();
      setShowAddModal(false);
      showToast('User added successfully', 'success');
    } catch (error) {
      console.log('Error adding user:', error);
      showToast('Failed to add user', 'error');
    }
  };


  const handleEditUser = async (updatedUser: User) => {
    try {
      await api.put(`/api/users/${updatedUser.id}`, { firstName: updatedUser.firstName, lastName: updatedUser.lastName, email: `${updatedUser.username}@unsa.ba`, facultyId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", indexNumber: updatedUser.indexNumber, role: Number(updatedUser.role), status: 1 });

      setUsers(users.map(u => u.id === updatedUser.id ? { ...updatedUser, role: ROLE_MAP[updatedUser.role as any] } : u));
      setSelectedUser(updatedUser);
      setShowEditModal(false);

      showToast('User updated successfully', 'success');

    } catch (error) {
      console.log('Error updating user:', error);
      showToast('Failed to update user', 'error');
    }
  };

  const handleDeleteUser = async () => {
    if (selectedUser) {
      await api.delete(`/api/users/${selectedUser.id}`);
      setUsers(users.filter(u => u.id !== selectedUser.id));
      setSelectedUser(null);
      setShowDeleteDialog(false);
      showToast('User deleted successfully', 'success');
    }
  };

  return (
    <CContainer fluid className="p-0 " style={{ minHeight: '100vh', background: '#f3f4f6', }}>
      <div className="d-flex flex-column" style={{ minHeight: '100vh' }}>
        <Header />
        <CContainer className="py-4">
          <CRow className="g-4">
            <CCol xs={12} lg={8}>
              <div>
                <SearchFilters
                  faculty={faculty}
                  setFaculty={setFaculty}
                  role={role}
                  setRole={setRole}
                  searchTerm={searchTerm}
                  setSearchTerm={setSearchTerm}
                  onAddUser={() => setShowAddModal(true)}
                />
                <div className="mt-3">
                  <UsersTable
                    users={filteredUsers}
                    selectedUser={selectedUser}
                    onSelectUser={setSelectedUser}
                    onEdit={() => setShowEditModal(true)}
                    onDelete={() => setShowDeleteDialog(true)}
                  />
                </div>
              </div>
            </CCol>
            <CCol xs={12} lg={4}>
              <UserDetailsPanel user={selectedUser} />
            </CCol>
          </CRow>
        </CContainer>

        {showAddModal && (
          <AddUserModal
            onClose={() => setShowAddModal(false)}
            onAdd={handleAddUser}
          />
        )}
        {showEditModal && selectedUser && (
          <EditUserModal
            user={selectedUser}
            onClose={() => setShowEditModal(false)}
            onSave={handleEditUser}
          />
        )}
        {showDeleteDialog && (
          <DeleteConfirmDialog
            user={selectedUser}
            onConfirm={handleDeleteUser}
            onCancel={() => setShowDeleteDialog(false)}
          />
        )}
        <ToastManager toast={toast} />
      </div>
    </CContainer>
  );

}
