import MyCourses from '../../component/MyCourses/MyCourses';
import UpcomingActivities from '../../component/UpcomingActivities/UpcomingActivities';

export default function DashboardPage() {
    return (
        <div className="page-container">
            <MyCourses />
            <UpcomingActivities />
        </div>
    )
}