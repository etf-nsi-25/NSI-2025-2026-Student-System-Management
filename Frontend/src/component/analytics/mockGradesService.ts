import { COLORS, type GradeSlice } from "../../page/analytics/ProfessorAnalytics";

export async function fetchGrades(filter: string): Promise<GradeSlice[]> {
  // Simulate backend delay
  await new Promise((r) => setTimeout(r, 200));

  // Different mock datasets depending on filter
  if (filter === 'final') {
    return [
      { label: '<6', value: 5, color: COLORS.low },
      { label: '6', value: 11, color: COLORS.six },
      { label: '7', value: 20, color: COLORS.seven },
      { label: '8', value: 9, color: COLORS.eight },
      { label: '9', value: 41, color: COLORS.nine },
      { label: '10', value: 52, color: COLORS.ten },
    ];
  }

  if (filter === 'midterm') {
    return [
      { label: '<6', value: 8, color: COLORS.low },
      { label: '6', value: 6, color: COLORS.six },
      { label: '7', value: 18, color: COLORS.seven },
      { label: '8', value: 15, color: COLORS.eight },
      { label: '9', value: 30, color: COLORS.nine },
      { label: '10', value: 29, color: COLORS.ten },
    ];
  }

  // default / all
  return [
    { label: '<6', value: 5, color: COLORS.low },
    { label: '6', value: 11, color: COLORS.six },
    { label: '7', value: 20, color: COLORS.seven },
    { label: '8', value: 9, color: COLORS.eight },
    { label: '9', value: 41, color: COLORS.nine },
    { label: '10', value: 52, color: COLORS.ten },
  ];
}
