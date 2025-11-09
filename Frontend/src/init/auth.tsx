import type { PropsWithChildren } from 'react';
import { createContext, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';

export interface AuthInfo {
    accessToken: string,
    expiresOn: Date,
    // TODO probably add user info like username here
}

export interface AuthContextData {
    authInfo?: AuthInfo,
    setAuthInfo: (authInfo: AuthInfo) => void;
}

const AuthContext = createContext<AuthContextData>({} as AuthContextData)

// TODO: use this to import this auth context into /login page, and use setAuthInfo after fetching from backend
// Then the initial data (default-token) should be removed
export function useAuthContext() {
    return useContext(AuthContext);
}

export function Authentication({ children }: PropsWithChildren<object>) {
    const [authInfo, setAuthInfo] = useState<AuthInfo>({ accessToken: 'default-token', expiresOn: new Date() })
    const navigate = useNavigate()

    useEffect(() => {
        const tokenValid = false
        if (!tokenValid) {
            // TODO once auth is implemented this should check for validity of token
            // and redirect to /login in case not valid (or attempt silent refresh via refresh token first)
            navigate('/page1')
        }
    }, [ authInfo ]);

    return (
        <AuthContext.Provider value={{ authInfo, setAuthInfo }}>
            { children }
        </AuthContext.Provider>
    )
}
