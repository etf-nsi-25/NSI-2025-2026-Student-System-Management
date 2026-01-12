export type Role = 'Professor' | 'Assistant' | 'Student' | 'Staff';
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

export let MOCK_USERS: User[] = [
    { id: 'u1', username: 'adnan', firstName: 'Prof. Adnan', lastName: 'Adnan', email: 'adnan.adnan@unsa.ba', role: 'Professor', facultyId: 'f1', status: 'Active', lastActive: 'Today, 09:15', indexNumber: undefined },
    { id: 'u2', username: 'lamija', firstName: 'Lamija', lastName: 'Salihović', email: 'lamija.s@unsa.ba', role: 'Student', facultyId: 'f1', status: 'Active', lastActive: 'Yesterday', indexNumber: '19045' },
    { id: 'u3', username: 'lejla', firstName: 'Lejla', lastName: 'Čelić', email: 'lejla@etf.unsa.ba', role: 'Assistant', facultyId: 'f1', status: 'Active', lastActive: '1 week ago', indexNumber: undefined },
    { id: 'u4', username: 'nejla', firstName: 'Nejla', lastName: 'Nejla', email: 'nejla@etf.unsa.ba', role: 'Staff', facultyId: 'f2', status: 'Active', lastActive: 'Today', indexNumber: undefined },
];

export const getAvailableFaculties = (): Faculty[] => [
    { id: 'f1', name: 'ETF UNSA' },
    { id: 'f2', name: 'EKOF UNSA' },
    { id: 'f3', name: 'MED UNSA' },
];

export const fetchUsers = async (): Promise<User[]> => {
    // eslint-disable-next-line no-console
    console.log('identityApi.fetchUsers: returning MOCK_USERS (count=', MOCK_USERS.length, ')');
    return new Promise(resolve => setTimeout(() => resolve(MOCK_USERS), 500));
};

type CreateUserPayload = Omit<User, 'id' | 'lastActive' | 'status'> & { password: string; status?: Status };

export const createUser = async (data: CreateUserPayload): Promise<User> => {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            // debug
            // eslint-disable-next-line no-console
            console.log('identityApi.createUser called with', data);

            if (MOCK_USERS.some(u => u.username === data.username)) {
                // eslint-disable-next-line no-console
                console.warn('identityApi.createUser: username already exists', data.username);
                return reject({ message: "Username je već zauzet." });
            }

            const newUser: User = {
                ...data,
                id: `u${MOCK_USERS.length + 1}`,
                lastActive: 'Never',
                status: (data as any).status || 'Active',
                indexNumber: (data as any).role === 'Student' ? (data as any).indexNumber : undefined,
            };
            MOCK_USERS.push(newUser);
            // eslint-disable-next-line no-console
            console.log('identityApi.createUser: pushed new user; MOCK_USERS length =', MOCK_USERS.length);
            resolve(newUser);
        }, 800);
    });
};

export const updateUser = async (
    id: string,
    data: Partial<Omit<User, 'id' | 'username' | 'email' | 'lastActive'>>
): Promise<User> => {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            const index = MOCK_USERS.findIndex(u => u.id === id);
            if (index === -1) {
                return reject({ message: "Korisnik nije pronađen." });
            }

            const updatedUser: User = {
                ...MOCK_USERS[index],
                ...data,
                status: (data as any).status || MOCK_USERS[index].status,
                indexNumber: data.role === 'Student' ? data.indexNumber : undefined,
            };

            MOCK_USERS[index] = updatedUser;
            resolve(updatedUser);
        }, 800);
    });
};


export const deleteUser = async (id: string): Promise<void> => {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            const initialLength = MOCK_USERS.length;
            MOCK_USERS = MOCK_USERS.filter(u => u.id !== id);
            if (MOCK_USERS.length < initialLength) {
                resolve();
            } else {
                reject({ message: "Korisnik nije pronađen za brisanje." });
            }
        }, 500);
    });
};

export const getFacultyName = (facultyId: string): string => {
    const faculty = getAvailableFaculties().find(f => f.id === facultyId);
    return faculty ? faculty.name : 'N/A';
};

export const deactivateUser = async (id: string): Promise<User> => {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            const index = MOCK_USERS.findIndex(u => u.id === id);
            if (index === -1) {
                return reject({ message: "Korisnik nije pronađen." });
            }
            MOCK_USERS[index] = { ...MOCK_USERS[index], status: 'Inactive' };
            resolve(MOCK_USERS[index]);
        }, 500);
    });
};