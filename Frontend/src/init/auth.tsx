import type { PropsWithChildren } from 'react';
import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router';
import { attemptSilentRefresh } from '../utils/authUtils.ts';
import { resetAuthInfo } from '../utils/authUtils.ts';

export interface AccessToken {
  iat: number;
  exp: number;
  jti: string;
  userId: string;
  email: string;
  role: string;
  tenantId: string;
  fullName: string;
}

export interface AuthInfo {
    accessToken: string,
    expiresOn: number,
    userId: string,
    email: string,
    role: string,
    tenantId: string,
    fullName: string,
}

export interface AuthContextData {
    authInfo?: AuthInfo,
    setAuthInfo: (authInfo: AuthInfo | undefined) => void;
}

const AuthContext = createContext<AuthContextData>({} as AuthContextData)

const getInitialAuthInfo = (): AuthInfo | undefined => {
    try {
        const stored = localStorage.getItem('authInfo');
        if (stored) {
            return JSON.parse(stored);
        }
    } catch (error) {
    }
    return undefined;
};

export function useAuthContext() {
    return useContext(AuthContext);
}

export function Authentication({ children }: PropsWithChildren<object>) {
    const [authInfo, setAuthInfoState] = useState<AuthInfo | undefined>(getInitialAuthInfo)
    const navigate = useNavigate();

    const setAuthInfo = useCallback((newAuthInfo: AuthInfo | undefined) => {
        setAuthInfoState(newAuthInfo);
        if(newAuthInfo) {
          localStorage.setItem('authInfo', JSON.stringify(newAuthInfo));
        }
    }, []);


    const refreshToken = useCallback(async () => {
        try {
            const newAuthInfo = await attemptSilentRefresh();

            setAuthInfo(newAuthInfo);
        } catch (error) {
            resetAuthInfo(setAuthInfoState);
            navigate('/login');
        }
    }, [navigate, setAuthInfo]);

    useEffect(() => {
        // If token is invalid or expired, attempt silent refresh before redirecting to login
        const isTokenExpired = authInfo?.expiresOn && Date.now() >= authInfo.expiresOn;

        if (isTokenExpired) {
            refreshToken();
        }
    }, [authInfo, refreshToken]);

    const contextValue: AuthContextData = useMemo(() => ({ authInfo, setAuthInfo }), [authInfo, setAuthInfo]);

    return (
        <AuthContext.Provider value={ contextValue }>
            { children }
        </AuthContext.Provider>
    )
}
