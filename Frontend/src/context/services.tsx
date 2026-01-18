/* eslint-disable react-refresh/only-export-components */

import type { PropsWithChildren } from "react";
import { createContext, useContext, useCallback, useMemo } from "react";
import { useAuthContext } from "../init/auth.tsx";
import { API } from "../api/api.ts";
import { RestClient } from "../api/rest.ts";
import { attemptSilentRefresh, resetAuthInfo } from "../utils/authUtils.ts";

export interface Services {
    api: API;
}

const ServiceContext = createContext<Services | null>(null);

export function useServiceContext(): Services {
    const ctx = useContext(ServiceContext);
    if (!ctx) {
        throw new Error("ServiceContextProvider is missing");
    }
    return ctx;
}

export function useAPI(): API {
    return useServiceContext().api;
}

export function ServiceContextProvider({ children }: PropsWithChildren<object>) {
    const authContextData = useAuthContext();

    const refreshToken = useCallback(async () => {
        try {
            const newAuthInfo = await attemptSilentRefresh();
            authContextData.setAuthInfo(newAuthInfo);
            return newAuthInfo;
        } catch {
            resetAuthInfo(authContextData.setAuthInfo);
        }
    }, [authContextData]);

    const value: Services = useMemo(
        () => ({
            api: new API(new RestClient(authContextData.authInfo!, refreshToken)),
        }),
        [authContextData.authInfo, refreshToken],
    );

    return (
        <ServiceContext.Provider value={value}>
            {children}
        </ServiceContext.Provider>
    );
}
