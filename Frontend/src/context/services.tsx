import type { PropsWithChildren } from 'react';
import { createContext, useContext } from 'react';
import { useAuthContext } from '../init/auth.tsx';
import { API } from '../service/api.ts';

export interface Services {
    api: API
}

const ServiceContext = createContext<Services>({} as Services)

export function useServiceContext() {
    return useContext(ServiceContext)
}

export function ServiceContextProvider({ children }: PropsWithChildren<object>) {
    const authContextData = useAuthContext()

    function refreshToken() {
        // TODO: attempt to refresh token via AuthService, if that fails redirect to /login
    }

    const value: Services = {
        // Since login API call is done without using API service, and all pages require login,
        // we can be sure auth info is initialized
        api: new API(authContextData.authInfo!, refreshToken)
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
