import { Navigate, useLocation } from 'react-router';
import { useAuthContext } from '../init/auth.tsx';
import { isAdmin, isAssistant, isStudent, isTeacher } from '../constants/roles.ts';

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: string[];
}

/**
 * ProtectedRoute component that checks if user is authenticated before rendering the protected page.
 * Redirects to login if user is not authenticated.
 * 
 * @param children - The component to render if authenticated
 * @param allowedRoles - Optional array of roles that can access this route
 */
export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { authInfo } = useAuthContext();
  const location = useLocation();

  // Not authenticated, redirect to login
  if (!authInfo) {
    return <Navigate to="/login" replace />;
  }

  // If specific roles are required, check them
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

    // Check if accessing student routes
  if (location.pathname.startsWith('/student') && !isStudent(authInfo.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Check if accessing teacher routes (future)
  if (location.pathname.startsWith('/teacher') && !isTeacher(authInfo.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Check if accessing admin routes (future)
  if (location.pathname.startsWith('/admin') && !isAdmin(authInfo.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
}
