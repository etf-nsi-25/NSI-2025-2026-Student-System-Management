import type { API } from '../api/api';
import type { UpcomingActivityDTO } from '../dto/UpcomingActivityDTO';

export async function getUpcomingActivities(api: API): Promise<UpcomingActivityDTO[]> {
    return api.getUpcomingActivities();
}
