import type { GradeResponse } from "../../dto/GradeDTO";

type Props = {
    grades: GradeResponse[];
    onGradeChange: (studentId: number, points: number | null) => void;
};

export default function GradesTable({ grades, onGradeChange }: Props) {
    const isValid = (points: number | null) => points === null || (points >= 0 && points <= 100);

    return (
        <table className="grades-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Points</th>
                    <th>Grade</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                {grades.map(g => (
                    <tr key={g.studentId}>
                        <td>{g.studentId}</td>
                        <td>{g.studentName}</td>
                        <td>
                            <input
                                type="number"
                                value={g.points ?? ""}
                                onChange={e => onGradeChange(g.studentId, e.target.value === "" ? null : Number(e.target.value))}
                                className={isValid(g.points) ? "" : "invalid"}
                            />
                        </td>
                        <td>{g.points !== null ? Math.round((g.points / 100) * 10) : "-"}</td>
                        <td>
                            {g.passed === null
                                ? "-"
                                : g.passed
                                ? <span className="badge pass">Pass</span>
                                : <span className="badge fail">Fail</span>}
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}
