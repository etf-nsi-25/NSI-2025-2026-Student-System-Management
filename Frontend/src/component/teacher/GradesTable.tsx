import type { GradeResponse } from "../../dto/GradeDTO";

type Props = {
    grades: GradeResponse[];
};

export default function GradesTable({ grades }: Props) {
    return (
        <>
            <h3>Exam Result</h3>

            <table className="grades-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Points</th>
                        <th>Status</th>
                    </tr>
                </thead>

                <tbody>
                    {grades.map(g => (
                        <tr key={g.studentId}>
                            <td>{g.studentId}</td>
                            <td>{g.studentName}</td>
                            <td>{g.points ?? "-"}</td>
                            <td>
                                {g.passed === null ? (
                                    "-"
                                ) : g.passed ? (
                                    <span className="badge pass">Pass</span>
                                ) : (
                                    <span className="badge fail">Fail</span>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}
