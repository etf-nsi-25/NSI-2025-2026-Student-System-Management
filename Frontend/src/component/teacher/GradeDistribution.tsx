import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';

type Props = {
    grades: { points: number | null; passed: boolean | null }[];
};

export default function GradeDistribution({ grades }: Props) {
    // Bucket grades by 5–10
    const buckets = [5, 6, 7, 8, 9, 10].map(grade => ({
        grade: grade.toString(),
        count: grades.filter(g => {
            if (g.points === null) return false;
            const p = g.points;
            if (p < 55) return grade === 5;
            if (p < 65) return grade === 6;
            if (p < 75) return grade === 7;
            if (p < 85) return grade === 8;
            if (p < 95) return grade === 9;
            return grade === 10;
        }).length
    }));

    return (
        <>
            <h3>Grade distribution</h3>
            <ResponsiveContainer width="100%" height="100%">
                <BarChart data={buckets}>
                    <XAxis dataKey="grade" />
                    <YAxis allowDecimals={false} />
                    <Tooltip />
                    <Bar
                        dataKey="count"
                        className="bar"
                        radius={[6, 6, 0, 0]} // rounded top edges
                    />
                </BarChart>
            </ResponsiveContainer>
        </>
    );
}
