import { useEffect, useState } from 'react';
import { CCard, CCardBody, CSpinner } from '@coreui/react';
import { useAPI } from '../../context/services.tsx';
import { getUpcomingActivities } from '../../service/upcomingActivitiesService';
import type { UpcomingActivityDTO } from '../../dto/UpcomingActivityDTO';
import './UpcomingActivities.css';

interface UpcomingActivitiesProps {
    className?: string;
}

export default function UpcomingActivities({ className }: UpcomingActivitiesProps) {
    const api = useAPI();
    const [activities, setActivities] = useState<UpcomingActivityDTO[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        void (async () => {
            setLoading(true);
            setError(null);
            try {
                const data = await getUpcomingActivities(api);
                setActivities(data);
            } catch (err) {
                console.error('Failed to fetch upcoming activities', err);
                setError('Failed to load upcoming activities');
            } finally {
                setLoading(false);
            }
        })();
    }, [api]);

    const formatTime = (time: string | null): string => {
        if (!time) return '';
        const [hours, minutes] = time.split(':');
        const hour = parseInt(hours, 10);
        const period = hour >= 12 ? 'P.M' : 'A.M';
        const displayHour = hour > 12 ? hour - 12 : hour === 0 ? 12 : hour;
        const displayMinutes = minutes || '00';
        return `${displayHour}:${displayMinutes} ${period}`;
    };

    const formatDate = (dateString: string): { day: number; month: string; year: number; fullDate: string } => {
        const date = new Date(dateString);
        const day = date.getDate();
        const month = date.toLocaleString('default', { month: 'long' });
        const year = date.getFullYear();
        const fullDate = `${month} ${day}, ${year}`;
        return { day, month, year, fullDate };
    };

    const isDueSoon = (dateString: string): boolean => {
        const date = new Date(dateString);
        const now = new Date();
        const diffTime = date.getTime() - now.getTime();
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return diffDays <= 7 && diffDays >= 0;
    };


    if (loading) {
        return (
            <div className={`upcoming-activities-section ${className || ''}`}>
                <h2 className="upcoming-activities-title">Upcoming activities</h2>
                <CCard className="upcoming-activities-card">
                    <CCardBody>
                        <div className="text-center p-4">
                            <CSpinner />
                        </div>
                    </CCardBody>
                </CCard>
            </div>
        );
    }

    if (error) {
        return (
            <div className={`upcoming-activities-section ${className || ''}`}>
                <h2 className="upcoming-activities-title">Upcoming activities</h2>
                <CCard className="upcoming-activities-card">
                    <CCardBody>
                        <div className="text-center text-danger p-4">
                            {error}
                        </div>
                    </CCardBody>
                </CCard>
            </div>
        );
    }

    if (activities.length === 0) {
        return (
            <div className={`upcoming-activities-section ${className || ''}`}>
                <h2 className="upcoming-activities-title">Upcoming activities</h2>
                <CCard className="upcoming-activities-card">
                    <CCardBody>
                        <div className="text-center text-muted p-4">
                            No upcoming activities
                        </div>
                    </CCardBody>
                </CCard>
            </div>
        );
    }

    return (
        <div className={`upcoming-activities-section ${className || ''}`}>
            <h2 className="upcoming-activities-title">Upcoming activities</h2>
            <CCard className="upcoming-activities-card">
                <CCardBody>
                    <div className="upcoming-activities-list">
                        {activities.map((activity, index) => {
                            const { day, fullDate } = formatDate(activity.date);
                            const timeDisplay = formatTime(activity.time);
                            const dueSoon = isDueSoon(activity.date);

                            return (
                                <div key={index} className="activity-item">
                                    <div className="activity-day-badge">
                                        <span className="day-number">{day}</span>
                                    </div>
                                    <div className="activity-content">
                                        <div className="activity-row">
                                            <h6 className="activity-title" title={activity.title}>{activity.title}</h6>
                                            {timeDisplay && (
                                                <div className="activity-time">
                                                    <span className="time-dot"></span>
                                                    {timeDisplay}
                                                </div>
                                            )}
                                        </div>
                                        <div className="activity-row">
                                            {(fullDate || activity.location) && (
                                                <div className="activity-date">
                                                    {fullDate}  
                                                    {activity.location && (
                                                        <span>, {activity.location}</span>
                                                    )}
                                                </div>
                                            )}
                                            <span className={`activity-status-badge ${dueSoon ? 'due-soon' : 'upcoming'}`}>
                                                {dueSoon ? 'Due Soon' : 'Upcoming'}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </CCardBody>
            </CCard>
        </div>
    );
}
