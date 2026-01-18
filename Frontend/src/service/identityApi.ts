import type { API } from '../api/api';

export type Role = 'Professor' | 'Assistant' | 'Student' | 'Admin' | 'Superadmin';
export type Status = 'Active' | 'Inactive';

export interface Faculty {
    id: string;
    name: string;
}

export interface User {
    id: string;
    username: string;
    firstName: string;
    lastName: string;
    email: string;
    role: Role;
    facultyId: string;
    status: Status;
    lastActive: string;
    indexNumber?: string;
}

export const getAvailableFaculties = (): Faculty[] => [
    { id: '11111111-1111-1111-1111-111111111111', name: 'ETF UNSA' },
    { id: 'f2', name: 'EKOF UNSA' },
    { id: 'f3', name: 'MED UNSA' },
];

// Mappings based on backend enums (Reverting to INTEGERS as verified by curl)
const mapFrontendRoleToBackend = (role: Role): number => {
    switch (role) {
        case 'Professor': return 3; // Teacher
        case 'Assistant': return 4;
        case 'Student': return 5;
        case 'Admin': return 2; // Admin
        case 'Superadmin': return 1; // Superadmin
        default: return 5;
    }
};

const mapFrontendStatusToBackend = (status: Status): number => {
    return status === 'Active' ? 1 : 2;
};

const mapBackendRole = (role: number | string): Role => {
    // Handle both int (if backend changes mind) and string
    if (typeof role === 'string') {
        if (role === 'Teacher') return 'Professor';
        if (role === 'Staff') return 'Admin'; // Graceful handling for old data if any
        return role as Role;
    }
    switch (role) {
        case 3: return 'Professor';
        case 4: return 'Assistant';
        case 5: return 'Student';
        case 1: return 'Superadmin';
        case 2: return 'Admin';
        default: return 'Admin';
    }
};

const mapBackendStatus = (status: number | string): Status => {
    if (typeof status === 'string') return status as Status;
    return status === 1 ? 'Active' : 'Inactive';
};

export const fetchUsers = async (api: API): Promise<User[]> => {
    // Backend returns { items: [...], totalCount: ... }
    const response: any = await api.get('/api/users');
    const items = response.items || [];

    return items.map((u: any) => ({
        id: u.id,
        username: u.username,
        firstName: u.firstName,
        lastName: u.lastName,
        email: u.email,
        role: mapBackendRole(u.role),
        facultyId: u.facultyId,
        status: mapBackendStatus(u.status),
        lastActive: 'Unknown', // Backend doesn't track this
        indexNumber: u.indexNumber
    }));
};

type CreateUserPayload = Omit<User, 'id' | 'lastActive' | 'status'> & { status?: Status };

export const createUser = async (api: API, data: CreateUserPayload): Promise<User> => {
    // Explicitly construct payload to ensure cleaner JSON and correct types
    const payload = {
        username: data.username,
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        facultyId: data.facultyId,
        // Backend expects specific string enum names: Teacher, Student, etc.
        role: mapFrontendRoleToBackend(data.role),
        indexNumber: data.indexNumber || null
    };

    // DEBUG: Log payload to see what we are sending
    // eslint-disable-next-line no-console
    console.log('Sending createUser payload:', JSON.stringify(payload, null, 2));

    // Backend returns the created UserResponse
    const response: any = await api.post('/api/users', payload);

    return {
        id: response.id,
        username: response.username,
        firstName: response.firstName,
        lastName: response.lastName,
        email: response.email,
        role: mapBackendRole(response.role),
        facultyId: response.facultyId,
        status: mapBackendStatus(response.status),
        lastActive: 'Just now',
        indexNumber: response.indexNumber
    };
};

export const updateUser = async (
    api: API,
    id: string,
    data: Partial<Omit<User, 'id' | 'lastActive'>>
): Promise<User> => {
    // Calls IdentityController.UpdateUser(string userId, [FromBody] UpdateUserRequest request)
    // Explicit construction for UpdateUserRequest
    const payload = {
        id: id,
        firstName: data.firstName,
        lastName: data.lastName,
        facultyId: data.facultyId,
        role: data.role ? mapFrontendRoleToBackend(data.role as Role) : undefined,
        // UserStatus Enum: Active=1, Inactive=2 (Integers verified)
        status: data.status ? mapFrontendStatusToBackend(data.status as Status) : undefined,
        indexNumber: data.indexNumber || null,
        email: data.email,
        username: data.username
    };

    await api.put(`/api/users/${id}`, payload);

    return { ...data, id } as User;
};


export const deleteUser = async (api: API, id: string): Promise<void> => {
    // Calls IdentityController.DeleteUser(string userId)
    await api.delete(`/api/users/${id}`);
};

export const getFacultyName = (facultyId: string): string => {
    const faculty = getAvailableFaculties().find(f => f.id === facultyId);
    return faculty ? faculty.name : 'N/A';
};

export const deactivateUser = async (api: API, id: string): Promise<User> => {
    // Calls IdentityController.DeactivateUser(string userId)
    await api.patch(`/api/users/${id}/deactivate`);
    // Similar to update, returns NoContent.
    return { id, status: 'Inactive' } as User;
};