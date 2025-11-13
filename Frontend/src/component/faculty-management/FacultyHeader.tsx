import React from 'react';
import './FacultyHeader.css';
import logo from '../../assets/images/logo.jpg';

export function FacultyHeader() {
    return (
        <header className="faculty-header">
            <div className="faculty-logo">
                <img src={logo} alt="Logo" />
            </div>
            <div className="faculty-label">
                University Admin
            </div>
        </header>
    );
}
