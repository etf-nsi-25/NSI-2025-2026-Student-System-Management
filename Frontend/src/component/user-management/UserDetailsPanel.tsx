import type { User } from "../../types/user-types";


interface UserDetailsPanelProps {
  user: User | null;
}

export default function UserDetailsPanel({ user }: UserDetailsPanelProps) {
  if (!user) {
    return (
      <div className="w-80 bg-white rounded-lg shadow p-6 flex items-center justify-center max-h-[400px]">
        <p className="text-gray-400 text-sm">Select a user to view details</p>
      </div>
    );
  }

  return (
    <div className="w-80 bg-white rounded-lg shadow p-6 flex flex-col gap-6 max-h-[400px]">
      <h2 className="text-lg font-bold text-blue-900">User Details</h2>

      <div>
        <label className="block text-xs font-semibold text-gray-600 mb-1">Name:</label>
        <p className="text-sm font-semibold text-gray-900">{user.name}</p>
      </div>

      <div>
        <label className="block text-xs font-semibold text-gray-600 mb-1">Role:</label>
        <p className="text-sm text-gray-700">{user.role}</p>
      </div>

      <div>
        <label className="block text-xs font-semibold text-gray-600 mb-1">Permissions:</label>
        <p className="text-sm text-gray-500">Edit Courses, Grade Students</p>
      </div>

      <div>
        <label className="block text-xs font-semibold text-gray-600 mb-1">Last login:</label>
        <p className="text-sm text-gray-700">{user.lastActive}, 09:15</p>
      </div>
      <div>
        <button className="bg-blue-900 text-white px-6 py-2 rounded text-sm font-semibold hover:bg-blue-800">
          Edit Permissions
        </button>
      </div>

    </div>
  );
}
