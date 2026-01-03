import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthContext } from '../init/auth';
import { isAdmin, isAssistant, isStudent, isTeacher } from '../constants/roles';

interface ProtectedRouteProps {
  allowedRoles?: string[];
}

/**
 * ProtectedRoute component that checks if user is authenticated
 * and authorized before rendering protected routes.
 */
export function ProtectedRoute({ allowedRoles }: ProtectedRouteProps) {
  const { authInfo } = useAuthContext();
  const location = useLocation();

  // Not authenticated → login
  if (!authInfo) {
    return <Navigate to="/login" replace />;
  }

  // Role-based access (if defined)
  if (allowedRoles && allowedRoles.length > 0) {
    const hasAccess = allowedRoles.some(role => {
      switch (role.toLowerCase()) {
        case 'student':
          return isStudent(authInfo.role);
        case 'teacher':
          return isTeacher(authInfo.role);
        case 'admin':
        case 'superadmin':
          return isAdmin(authInfo.role);
        case 'assistant':
          return isAssistant(authInfo.role);
        default:
          return false;
      }
    });

    if (!hasAccess) {
      return <Navigate to="/unauthorized" replace />;
    }
  }

  // Path-based protection (existing logic stays)
  if (location.pathname.startsWith('/student') && !isStudent(authInfo.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (location.pathname.startsWith('/teacher') && !isTeacher(authInfo.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (location.pathname.startsWith('/admin') && !isAdmin(authInfo.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <Outlet />;
}
