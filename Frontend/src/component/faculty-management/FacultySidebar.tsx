import React from 'react';
import { Link } from 'react-router';
import './FacultySidebar.css';

export function FacultySidebar() {
    return (
        <aside className="faculty-sidebar">
            <nav>
                <ul>
                    <li>
                        <Link to="/dashboard">Dashboard</Link>
                    </li>
                    <li>
                        <Link to="/help">Help</Link>
                    </li>
                    <li>
                        <Link to="/logout">Logout</Link>
                    </li>
                </ul>
            </nav>
        </aside>
    );
}
