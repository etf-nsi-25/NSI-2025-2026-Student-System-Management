import { UserRole } from "./roles";

export type SidebarItem = {
  label: string;
  path: string;
  roles: UserRole[];
};

export const sidebarItems: SidebarItem[] = [
  {
    label: "Dashboard",
    path: "/dashboard",
    roles: [
      UserRole.Superadmin,
      UserRole.Admin,
      UserRole.Teacher,
      UserRole.Assistant,
      UserRole.Student,
    ],
  },
  {
    label: "Course Management",
    path: "/course-management",
    roles: [
      UserRole.Superadmin,
      UserRole.Admin,
      UserRole.Teacher,
    ],
  },
  {
    label: "User Management",
    path: "/users",
    roles: [UserRole.Superadmin, UserRole.Admin],
  },
  {
    label: "Tenant Management",
    path: "/tenant-management",
    roles: [UserRole.Superadmin],
  },
  {
    label: "Student Support",
    path: "/student-support",
    roles: [
      UserRole.Admin,
      UserRole.Teacher,
      UserRole.Assistant,
    ],
  },
  {
    label: "Settings",
    path: "/settings",
    roles: [UserRole.Superadmin, UserRole.Admin],
  },
  {
    label: "Help",
    path: "/help",
    roles: [
      UserRole.Superadmin,
      UserRole.Admin,
      UserRole.Teacher,
      UserRole.Assistant,
      UserRole.Student,
    ],
  },
];
