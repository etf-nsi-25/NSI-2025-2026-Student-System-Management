import { API_BASE_URL } from '../constants/constants.ts';
import type { AuthInfo, AccessToken } from '../init/auth.tsx';
import { jwtDecode } from 'jwt-decode';

interface RefreshResponse {
  accessToken: string;
  tokenType: string;
}

/**
 * Attempts to refresh the access token using the refresh token stored in HTTP-only cookie
 * @returns Updated AuthInfo with new access token
 * @throws Error if refresh fails
 */
export async function attemptSilentRefresh(): Promise<AuthInfo> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include', // Include HTTP-only cookies
    });

    if (!response.ok) {
      console.warn('Silent refresh failed:', response.status);
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
  }
};

export const resetAuthInfo = (setAuthInfoState: (authInfo?: AuthInfo) => void): void => {
   // Reset auth info
    localStorage.removeItem('authInfo');
    setAuthInfoState(undefined);
};
