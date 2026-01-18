import type { API } from '../api/api';
import type { ProfessorCourseDTO } from '../dto/ProfessorCourseDTO';

export async function getProfessorCourses(api: API): Promise<ProfessorCourseDTO[]> {
    return api.getProfessorCourses();
}
