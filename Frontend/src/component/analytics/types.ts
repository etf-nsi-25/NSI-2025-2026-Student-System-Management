export interface CourseStatsDto {
  courseId: string;
  distribution: Record<string, number>;
  totalCount: number;
  passedCount: number;
}

export interface AnalyticsSummaryMetrics {
  enrolled: number;
  repeating: number; // Note: Backend does not provide this yet, will mock or set to 0
  avgGrade: number;
  passRate: number;
  avgAttendance: number;
}