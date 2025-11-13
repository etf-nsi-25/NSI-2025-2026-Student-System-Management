import React from 'react';
import { FacultyHeader } from '../../component/faculty-management/FacultyHeader';
import { FacultySidebar } from '../../component/faculty-management/FacultySidebar';
import './FacultyManagementPage.css';
import { FacultyListingPage } from './FacultyListingPage';

export function FacultyManagementPage() {
    return (
        <div className="faculty-page">
            <FacultyHeader />
            
            <div className="faculty-body">
                <FacultySidebar />
                <div className="faculty-content">
                    <FacultyListingPage />

                </div>
            </div>
        </div>
    );
}
