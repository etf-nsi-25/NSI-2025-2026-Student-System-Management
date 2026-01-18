import type { AuthInfo, AccessToken } from '../init/auth.tsx';
import { jwtDecode } from 'jwt-decode';

interface RefreshResponse {
  accessToken: string;
  tokenType: string;
  forcePasswordChange: boolean;
}

interface TwoFactorLoginRequiredResponse {
  requiresTwoFactor: true;
  twoFactorToken: string;
  forcePasswordChange: boolean;
}

interface LoginOkResponse {
  requiresTwoFactor?: false;
  accessToken: string;
  tokenType: string;
  forcePasswordChange: boolean;
}

type LoginResponse = LoginOkResponse | TwoFactorLoginRequiredResponse;

export type LoginResult =
  | { kind: '2fa'; twoFactorToken: string }
  | { kind: 'ok'; authInfo: AuthInfo };

// This serves as a global lock to prevent multiple invocations of access token refresh to invalidate
// each other's refresh tokens
let refreshInProgress: Promise<AuthInfo> | null = null;

/**
 * Attempts to refresh the access token using the refresh token stored in HTTP-only cookie
 * @returns Updated AuthInfo with new access token
 * @throws Error if refresh fails
 */

export async function loginWithCredentials(email: string, password: string): Promise<LoginResult> {
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
    const data: LoginResponse = await response.json();

    if ('requiresTwoFactor' in data && data.requiresTwoFactor) {
      return { kind: '2fa', twoFactorToken: data.twoFactorToken };
    }

    if (!data.accessToken) {
      throw new Error('Login failed');
    }
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
      forcePasswordChange: data.forcePasswordChange
    };
    return { kind: 'ok', authInfo };
  } catch (error) {

    console.error('Login error:', error);
    throw error;
  }
};

export async function verifyTwoFactorLogin(twoFactorToken: string, code: string): Promise<AuthInfo> {
  const response = await fetch(`/api/auth/verify-2fa`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ twoFactorToken, code }),
    credentials: 'include',
  });

  if (!response.ok) {
    console.warn('2FA verification failed:', response.status);
    throw new Error('2FA verification failed');
  }

  const data: RefreshResponse = await response.json();

  const decoded = jwtDecode<AccessToken>(data.accessToken);
  return {
    accessToken: data.accessToken,
    expiresOn: decoded.exp * 1000,
    email: decoded.email,
    userId: decoded.userId,
    role: decoded.role,
    tenantId: decoded.tenantId,
    fullName: decoded.fullName,
    forcePasswordChange: data.forcePasswordChange
  };
}

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
        forcePasswordChange: data.forcePasswordChange
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
