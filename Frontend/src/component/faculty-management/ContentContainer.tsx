import React from 'react';
import './ContentContainer.css';

interface ContentContainerProps {
    children: React.ReactNode;
}

export function ContentContainer({ children }: ContentContainerProps) {
    return (
        <div className="content-container">
            {children}
        </div>
    );
}
