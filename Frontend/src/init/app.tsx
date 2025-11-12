import { BrowserRouter } from 'react-router';
import { DummyContextProvider } from '../context/dummy/dummy-context.tsx';
import { ServiceContextProvider } from '../context/services.tsx';
import { Authentication } from './auth.tsx';
import { Router } from './router.tsx';

export function App() {
    return (
        <div>
            <BrowserRouter>
                <Authentication>
                    <ServiceContextProvider>
                        <DummyContextProvider>
                            <Router />
                        </DummyContextProvider>
                    </ServiceContextProvider>
                </Authentication>
            </BrowserRouter>
        </div>
    )
}
