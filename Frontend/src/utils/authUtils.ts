import type { AuthInfo, AccessToken } from '../init/auth.tsx';
import { jwtDecode } from 'jwt-decode';

interface RefreshResponse {
  accessToken: string;
  tokenType: string;
}

// This serves as a global lock to prevent multiple invocations of access token refresh to invalidate
// each other's refresh tokens
let refreshInProgress: Promise<AuthInfo> | null = null;

/**
 * Attempts to refresh the access token using the refresh token stored in HTTP-only cookie
 * @returns Updated AuthInfo with new access token
 * @throws Error if refresh fails
 */

export async function loginWithCredentials(email: string, password: string): Promise<AuthInfo> {
  try {
    const response = await fetch(`/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
      credentials: 'include', // Include HTTP-only cookies
    });
    if (!response.ok) {
      console.warn('Login failed:', response.status);
      throw new Error('Login failed');
    }
    const data : RefreshResponse = await response.json();
    // Decode JWT to extract claims
    const decoded = jwtDecode<AccessToken>(data.accessToken);
    const authInfo: AuthInfo = {
      accessToken: data.accessToken,
      expiresOn: decoded.exp * 1000,
      email: decoded.email,
      userId: decoded.userId,
      role: decoded.role,
      tenantId: decoded.tenantId,
      fullName: decoded.fullName,
    };
    return authInfo;
  } catch (error) {

    console.error('Login error:', error);
    throw error;
  }
};

export async function logoutFromServer(): Promise<void> {
  try {
    const response = await fetch(`/api/auth/logout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include', // Include HTTP-only cookies
    });

    console.log('Logout response status:', response.status);

    if (!response.ok) {
      console.warn('Logout failed:', response.status);
      throw new Error('Logout failed');
    }
  } catch (error) {
    console.error('Logout error:', error);
    throw error;
  }
}

export async function attemptSilentRefresh(): Promise<AuthInfo> {
  if (refreshInProgress) {
    return refreshInProgress;
  }

  refreshInProgress = (async () => {
    try {
      const response = await fetch(`/api/auth/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
      });

      if (!response.ok) {
        throw new Error('Token refresh failed');
      }

      const data : RefreshResponse = await response.json();

      // Decode JWT to extract claims
      const decoded = jwtDecode<AccessToken>(data.accessToken);
      const authInfo: AuthInfo = {
        accessToken: data.accessToken,
        expiresOn: decoded.exp * 1000,
        email: decoded.email,
        userId: decoded.userId,
        role: decoded.role,
        tenantId: decoded.tenantId,
        fullName: decoded.fullName,
      };

      return authInfo;
    } catch (error) {
      console.error('Silent refresh error:', error);
      throw error;
    } finally {
      // Clear the lock
      refreshInProgress = null;
    }
  })();

  return refreshInProgress;
}

export const resetAuthInfo = (setAuthInfoState: (authInfo?: AuthInfo) => void): void => {
   // Reset auth info
    localStorage.removeItem('authInfo');
    setAuthInfoState(undefined);
};
