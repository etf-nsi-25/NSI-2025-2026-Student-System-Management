import React from 'react';
import { CBadge } from '@coreui/react';
import { renderStatus } from '../utils/requestUtils';
import type { RequestStatus } from '../utils/requestUtils';

interface StatusBadgeProps {
    status: RequestStatus;
    className?: string;
}

const StatusBadge: React.FC<StatusBadgeProps> = ({ status, className }) => {
    return (
        <CBadge color={renderStatus(status)} className={className}>
            {status}
        </CBadge>
    );
};

export default StatusBadge;