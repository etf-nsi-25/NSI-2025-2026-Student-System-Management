import type { PropsWithChildren } from 'react';
import { createContext, useContext, useState } from 'react';

export interface DummyContextData {
    data: string,
    setData: (data: string) => void;
}

const DummyContext = createContext<DummyContextData>({} as DummyContextData)

export function useDummyContext() {
    return useContext(DummyContext);
}

export function DummyContextProvider({ children }: PropsWithChildren<object>) {
    const [contextState, setContextState] = useState<string>('');

    const value: DummyContextData = {
        data: contextState,
        setData: (data) => setContextState(data)
    }

    return (
        <DummyContext.Provider value={ value }>
            { children }
        </DummyContext.Provider>
    )
}
