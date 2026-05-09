/**
 * StatisticsPanel - Displays habit statistics
 * Shows: currentStreak, longestStreak, completionRate (as percentage)
 */

import React from 'react';
import type { HabitStatistics } from '../types/index';

interface StatisticsPanelProps {
  statistics: HabitStatistics;
}

export function StatisticsPanel({ statistics }: StatisticsPanelProps): React.JSX.Element {
  const completionRatePercent = (statistics.CompletionRate * 100).toFixed(0);

  return (
    <div className="d-flex gap-3 mt-4">
      <div className="tessera-stat-card flex-fill">
        <p className="stat-label">Current Streak</p>
        <p className="stat-value">{statistics.CurrentStreak} days</p>
      </div>

      <div className="tessera-stat-card flex-fill">
        <p className="stat-label">Longest Streak</p>
        <p className="stat-value">{statistics.LongestStreak} days</p>
      </div>

      <div className="tessera-stat-card flex-fill">
        <p className="stat-label">Completion Rate</p>
        <p className="stat-value">{completionRatePercent}%</p>
      </div>
    </div>
  );
}
