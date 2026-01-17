export const UserRole = {
  Superadmin: 'Superadmin',
  Admin: 'Admin',
  Teacher: 'Teacher',
  Assistant: 'Assistant',
  Student: 'Student',
} as const;

export type UserRole = typeof UserRole[keyof typeof UserRole];

export const UserRoleValue = {
  Superadmin: 1,
  Admin: 2,
  Teacher: 3,
  Assistant: 4,
  Student: 5,
} as const;

export type UserRoleValue = typeof UserRoleValue[keyof typeof UserRoleValue];


export function isRole(userRole: string, expectedRole: UserRole): boolean {
  return userRole.toLowerCase() === expectedRole.toLowerCase();
}

// Helper function to check if user is student
export function isStudent(userRole: string): boolean {
  return isRole(userRole, UserRole.Student);
}

// Helper function to check if user is admin or superadmin
export function isAdmin(userRole: string): boolean {
  return isRole(userRole, UserRole.Admin) || isRole(userRole, UserRole.Superadmin);
}

// Helper function to check if user is teacher
export function isTeacher(userRole: string): boolean {
  return isRole(userRole, UserRole.Teacher);
}

// Helper function to check if user is assistant
export function isAssistant(userRole: string): boolean {
  return isRole(userRole, UserRole.Assistant);
}

// Get dashboard route based on role
export function getDashboardRoute(userRole: string): string {
  const role = userRole.toLowerCase();

  switch (role) {
    case 'student':
      return '/student/dashboard';
    case 'admin':
    case 'superadmin':
      return '/admin/dashboard';
    case 'teacher':
      return '/teacher/dashboard';
    case 'assistant':
      return '/assistant/dashboard';
    default:
      return '/login';
  }
}