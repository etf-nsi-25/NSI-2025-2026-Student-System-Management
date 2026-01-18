import '../../page/analytics/ProfessorAnalytics.css';
import type { GradeSlice } from '../../page/analytics/ProfessorAnalytics';

type Props = {
  data: GradeSlice[];
  size?: number;
};

function polarToCartesian(cx: number, cy: number, r: number, angleDeg: number) {
  const angleRad = ((angleDeg - 90) * Math.PI) / 180.0;
  return { x: cx + r * Math.cos(angleRad), y: cy + r * Math.sin(angleRad) };
}

function describeArc(cx: number, cy: number, r: number, startAngle: number, endAngle: number) {
  const start = polarToCartesian(cx, cy, r, endAngle);
  const end = polarToCartesian(cx, cy, r, startAngle);
  const largeArcFlag = endAngle - startAngle <= 180 ? '0' : '1';
  return ['M', cx, cy, 'L', start.x, start.y, 'A', r, r, 0, largeArcFlag, 0, end.x, end.y, 'Z'].join(' ');
}

export const GradesOverview: React.FC<Props> = ({ data, size = 520 }) => {
  const total = data.reduce((s, d) => s + d.value, 0) || 1;
  let startAngle = 0;
  const cx = size / 2;
  const cy = size / 2 - 10;
  const r = Math.min(cx, cy) - 40;

  return (
    <div className="analytics-card">
      <div className="chart-row">
        <svg width={size} height={size} className="grades-svg">
          <defs>
            <filter id="shadow" x="-20%" y="-20%" width="140%" height="140%">
              <feDropShadow dx="0" dy="8" stdDeviation="10" floodColor="#000" floodOpacity="0.12" />
            </filter>
          </defs>

          <g filter="url(#shadow)">
            {data.map((slice) => {
              const valueAngle = (slice.value / total) * 360;
              const endAngle = startAngle + valueAngle;
              const path = describeArc(cx, cy, r, startAngle, endAngle);

              const midAngle = startAngle + valueAngle / 2;
              const labelPos = polarToCartesian(cx, cy, r + 28, midAngle);
              const linePos = polarToCartesian(cx, cy, r + 8, midAngle);

              startAngle = endAngle;

              return (
                <g key={slice.label} className="slice-group">
                  <path d={path} fill={slice.color} />
                  <line x1={polarToCartesian(cx, cy, r, midAngle).x} y1={polarToCartesian(cx, cy, r, midAngle).y} x2={linePos.x} y2={linePos.y} stroke={slice.color} strokeWidth={2} />
                  <text x={labelPos.x} y={labelPos.y} fill={slice.color} fontSize={18} textAnchor={labelPos.x < cx ? 'end' : 'start'} dy="6">
                    {slice.value}
                  </text>
                </g>
              );
            })}
          </g>
        </svg>

        <div className="legend-and-controls">
          <div className="legend">
            {data.map((d) => (
              <div className="legend-row" key={d.label}>
                <span className="legend-color" style={{ background: d.color }} />
                <span className="legend-label">{d.label}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default GradesOverview;
