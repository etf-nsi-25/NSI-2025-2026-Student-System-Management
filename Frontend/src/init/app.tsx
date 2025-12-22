import { BrowserRouter } from 'react-router';
import { DummyContextProvider } from '../context/dummy/dummy-context.tsx';
import { ServiceContextProvider } from '../context/services.tsx';
import { Authentication } from './auth.tsx';
import { Router } from './router.tsx';
import AppLayout from '../component/AppLayout/AppLayout.tsx';
import { ToastProvider } from '../context/toast.tsx';


export function App() {
    return (
        <div>
            <BrowserRouter>
                <Authentication>
                    <ServiceContextProvider>
                        <ToastProvider>
                            <DummyContextProvider>
                                <AppLayout>
                                    <Router />
                                </AppLayout>
                            </DummyContextProvider>
                        </ToastProvider>
                    </ServiceContextProvider>
                </Authentication>
            </BrowserRouter>
        </div>
    )
}
