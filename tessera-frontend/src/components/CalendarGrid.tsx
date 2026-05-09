/**
 * CalendarGrid - GitHub contribution graph-style calendar view
 * Weekly columns, daily squares
 * Completed day → filled square in habit's color
 * Incomplete day → grey/empty square
 */

import React from 'react';
import type { HabitCompletion } from '../types/index';

interface CalendarGridProps {
  habitColor: string;
  completions: HabitCompletion[];
}

export function CalendarGrid({
  habitColor,
  completions,
}: CalendarGridProps): React.JSX.Element {
  // Get all unique dates from completions (deduplicate)
  const completedDates = new Set(completions.map((c) => c.Date));

  // Calculate date range: 84 days (12 weeks)
  const today = new Date();
  const startDate = new Date(today);
  startDate.setDate(startDate.getDate() - 83); // 84 days total including today

  // Generate array of 84 dates in YYYY-MM-DD format
  const dates: string[] = [];
  for (let i = 0; i < 84; i++) {
    const date = new Date(startDate);
    date.setDate(date.getDate() + i);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    dates.push(`${year}-${month}-${day}`);
  }

  // Group dates into weeks (columns of 7)
  const weeks: string[][] = [];
  for (let i = 0; i < dates.length; i += 7) {
    weeks.push(dates.slice(i, i + 7));
  }

  return (
    <div className="d-flex flex-row gap-1" style={{ overflow: 'auto' }}>
      {weeks.map((week, weekIndex) => (
        <div key={weekIndex} className="d-flex flex-column gap-1">
          {week.map((date) => {
            const isCompleted = completedDates.has(date);
            const backgroundColor = isCompleted ? habitColor : '#2d3748';

            return (
              <div
                key={date}
                title={date}
                style={{
                  width: '14px',
                  height: '14px',
                  backgroundColor,
                  borderRadius: '2px',
                  cursor: 'pointer',
                }}
              />
            );
          })}
        </div>
      ))}
    </div>
  );
}
