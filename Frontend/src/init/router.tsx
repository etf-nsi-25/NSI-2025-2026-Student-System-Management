import React from 'react';
import { Route, Routes } from 'react-router';
import { Home } from '../page/home/home.tsx';
import { Page1 } from '../page/page1/page1.tsx';
import { Login } from '../page/login/login.tsx';

export function Router(): React.ReactNode {
    return (
        <Routes>
            <Route path="/" element={ <Home /> } />
            <Route path="/page1" element={ <Page1 /> } />
            <Route path="/login" element={ <Login /> } />
        </Routes>
    )
}
