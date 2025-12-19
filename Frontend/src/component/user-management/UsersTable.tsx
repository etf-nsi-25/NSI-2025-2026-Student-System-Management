import type { User } from "../../types/user-types";
import { CTable, CButton } from '@coreui/react';

interface UsersTableProps {
  users: User[];
  selectedUser: User | null;
  onSelectUser: (user: User) => void;
  onEdit: () => void;
  onDelete: () => void;
}

export default function UsersTable({
  users,
  selectedUser,
  onSelectUser,
  onEdit,
  onDelete,
}: UsersTableProps) {
  return (
    <div className="card p-3">
      <CTable responsive small hover>
        <thead>
          <tr>
            <th>Name</th>
            <th>Username</th>
            <th>Role</th>
            <th>Faculty</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr
              key={user.id}
              onClick={() => onSelectUser(user)}
              style={{ cursor: 'pointer', background: selectedUser?.id === user.id ? '#e9f2ff' : undefined }}
            >
              <td>{user.firstName + " " + user.lastName}</td>
              <td>{user.username}</td>
              <td>{user.role}</td>
              <td>{user.faculty}</td>
              <td>
                <CButton
                  color="secondary"
                  size="sm"
                  className="me-2"
                  onClick={(e: any) => {
                    e.stopPropagation();
                    onSelectUser(user);
                    onEdit();
                  }}
                >
                  Edit
                </CButton>
                <CButton
                  color="light"
                  size="sm"
                  onClick={(e: any) => {
                    e.stopPropagation();
                    onSelectUser(user);
                    onDelete();
                  }}
                >
                  Delete
                </CButton>
              </td>
            </tr>
          ))}
        </tbody>
      </CTable>

      <div className="d-flex gap-2 mt-3">
        <CButton color="primary">Save attendance</CButton>
        <CButton color="secondary">Export report</CButton>
      </div>
    </div>
  );
}
