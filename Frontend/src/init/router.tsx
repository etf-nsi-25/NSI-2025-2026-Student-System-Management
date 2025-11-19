import React from 'react';
import { Route, Routes } from 'react-router';
import { Home } from '../page/home/home';
import { Page1 } from '../page/page1/page1';
import CourseListPage from '../page/university/courses/CourseListPage';

export function Router(): React.ReactNode {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/page1" element={<Page1 />} />
            <Route path="/university/courses" element={<CourseListPage />} />
        </Routes>
    );
}
