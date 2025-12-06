import { Navigate } from 'react-router';
import { useAuthContext } from '../init/auth.tsx';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

/**
 * ProtectedRoute component that checks if user is authenticated before rendering the protected page.
 * Redirects to login if user is not authenticated.
 * 
 * @param children - The component to render if authenticated
 */
export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { authInfo } = useAuthContext();

  // Not authenticated, redirect to login
  if (!authInfo) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
}
