import type { GradeResponse } from "../../dto/GradeDTO";
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer
} from "recharts";

type Props = {
    grades: GradeResponse[];
};

export default function GradeDistribution({ grades }: Props) {
    const distribution = [
        { range: "0–49", count: 0 },
        { range: "50–64", count: 0 },
        { range: "65–79", count: 0 },
        { range: "80–100", count: 0 }
    ];

    grades.forEach(g => {
        if (g.points === null) return;

        if (g.points < 50) distribution[0].count++;
        else if (g.points < 65) distribution[1].count++;
        else if (g.points < 80) distribution[2].count++;
        else distribution[3].count++;
    });

    return (
        <>
            <h3>Grade distribution</h3>

            <div style={{ width: "100%", height: 250 }}>
                <ResponsiveContainer>
                    <BarChart data={distribution}>
                        <XAxis dataKey="range" />
                        <YAxis allowDecimals={false} />
                        <Tooltip />
                        <Bar dataKey="count" />
                    </BarChart>
                </ResponsiveContainer>
            </div>
        </>
    );
}
