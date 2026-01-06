import type { GradeResponse } from "../../dto/GradeDTO";

type Props = {
    grades: GradeResponse[];
};

export default function PerformanceSummary({ grades }: Props) {
    const taken = grades.filter(g => g.points !== null);
    const passed = grades.filter(g => g.passed === true);

    const average =
        taken.reduce((sum, g) => sum + (g.points ?? 0), 0) /
        (taken.length || 1);

    return (
        <>
            <h3>Students performance summary</h3>
            <ul className="summary-list">
                <li>Students who took the exam: <b>{taken.length}</b></li>
                <li>Students who passed: <b>{passed.length}</b></li>
                <li>Average score: <b>{Math.round(average)}%</b></li>
            </ul>
        </>
    );
}
