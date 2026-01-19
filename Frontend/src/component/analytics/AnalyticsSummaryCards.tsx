import React from 'react';
import type { AnalyticsSummaryMetrics } from './types';

interface Props {
  metrics: AnalyticsSummaryMetrics;
  isLoading: boolean;
}

export const AnalyticsSummaryCards: React.FC<Props> = ({ metrics, isLoading }) => {
  if (isLoading) {
    return (
      <aside className="paStatsCol">
        <div className="paStatCard">
            <div className="paStatLabel">Loading...</div>
        </div>
      </aside>
    );
  }

  return (
    <aside className="paStatsCol">
      <div className="paStatCard">
        <div className="paStatValue">{metrics.enrolled}</div>
        <div className="paStatLabel">ENROLLED</div>
      </div>
      <div className="paStatCard">
        <div className="paStatValue">{metrics.repeating}</div>
        <div className="paStatLabel">REPEATING</div>
      </div>
      <div className="paStatCard">
        <div className="paStatValue">{metrics.avgGrade.toFixed(1)}</div>
        <div className="paStatLabel">AVG GRADE</div>
      </div>
      <div className="paStatCard">
        <div className="paStatValue">{metrics.passRate.toFixed(0)}%</div>
        <div className="paStatLabel">PASSED</div>
      </div>
      <div className="paStatCard">
        <div className="paStatValue">{metrics.avgAttendance.toFixed(0)}%</div>
        <div className="paStatLabel">AVG ATTENDANCE</div>
      </div>
    </aside>
  );
};