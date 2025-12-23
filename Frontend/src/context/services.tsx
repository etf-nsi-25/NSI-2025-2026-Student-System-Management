import type { PropsWithChildren } from 'react';
import { createContext, useContext, useCallback } from 'react';
import { useAuthContext } from '../init/auth.tsx';
import { API } from '../api/api.ts';
import { RestClient } from '../api/rest.ts';
import { attemptSilentRefresh, resetAuthInfo } from '../utils/authUtils.ts';

export interface Services {
    api: API
}

const ServiceContext = createContext<Services>({} as Services)

export function useServiceContext() {
    return useContext(ServiceContext)
}

export function ServiceContextProvider({ children }: PropsWithChildren<object>) {
    const authContextData = useAuthContext()

    const refreshToken = useCallback(async () => {
        try {
            const newAuthInfo = await attemptSilentRefresh();
            authContextData.setAuthInfo(newAuthInfo);

            return newAuthInfo;
        } catch (error) {
            resetAuthInfo(authContextData.setAuthInfo);
        }
    }, [authContextData]);

    const value: Services = {
        // Since login API call is done without using API service, and all pages require login,
        // we can be sure auth info is initialized
        api: new API(new RestClient(authContextData.authInfo!, refreshToken))
    }

    return (
        <ServiceContext.Provider value={ value }>
            { children }
        </ServiceContext.Provider>
    )
}

export function useAPI() {
    return useServiceContext().api
}
