import type { User } from "../../types/user-types";


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
    <div className="bg-white rounded-lg shadow p-6 overflow-hidden">
      <table className="w-full text-sm  border border-gray-200 rounded-lg">
        <thead className="bg-gray-100 rounded-lg">
          <tr className="border-b border-gray-200">
            <th className="text-left font-semibold py-3 px-4">Name</th>
            <th className="text-left font-semibold py-3 px-4">Email</th>
            <th className="text-left font-semibold py-3 px-4">Role</th>
            <th className="text-left font-semibold py-3 px-4">Last Active</th>
            <th className="text-left font-semibold py-3 px-4">Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr
              key={user.id}
              className={`border-b border-gray-200 cursor-pointer ${
                selectedUser?.id === user.id ? 'bg-blue-50' : 'hover:bg-gray-50'
              }`}
              onClick={() => onSelectUser(user)}
            >
              <td className="py-3 px-4">{user.name}</td>
              <td className="py-3 px-4">{user.email}</td>
              <td className="py-3 px-4">{user.role}</td>
              <td className="py-3 px-4">{user.lastActive}</td>
              <td className="py-3 px-4 flex gap-2">
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onSelectUser(user);
                    onEdit();
                  }}
                  className="bg-blue-100 text-blue-900 px-3 py-1 rounded text-xs font-semibold hover:bg-blue-200"
                >
                  Edit
                </button>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onSelectUser(user);
                    onDelete();
                  }}
                  className="bg-gray-200 text-gray-900 px-3 py-1 rounded text-xs font-semibold hover:bg-gray-300"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="flex gap-3 mt-6">
        <button className="bg-blue-900 text-white px-6 py-2 rounded text-sm font-semibold hover:bg-blue-800">
          Save attendance
        </button>
        <button className="bg-gray-400 text-white px-6 py-2 rounded text-sm font-semibold hover:bg-gray-500">
          Export report
        </button>
      </div>
    </div>
  );
}
