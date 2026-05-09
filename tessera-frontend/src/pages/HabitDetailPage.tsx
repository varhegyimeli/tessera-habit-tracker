/**
 * HabitDetailPage - Detail view for a single habit
 * Displays: CalendarGrid, StatisticsPanel, CheckInButton
 * Route: /habits/:habitId
 */

import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import type { Habit, HabitCompletion, HabitStatistics } from '../types/index';
import { getHabits } from '../api/habitsApi';
import { getCompletions, getStatistics } from '../api/completionsApi';
import { CalendarGrid } from '../components/CalendarGrid';
import { StatisticsPanel } from '../components/StatisticsPanel';

export function HabitDetailPage(): React.JSX.Element {
  const { habitId } = useParams<{ habitId: string }>();
  const navigate = useNavigate();
  const [habit, setHabit] = useState<Habit | null>(null);
  const [completions, setCompletions] = useState<HabitCompletion[]>([]);
  const [statistics, setStatistics] = useState<HabitStatistics | null>(null);
  const [allHabits, setAllHabits] = useState<Habit[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!habitId) return;

    const fetchData = async (): Promise<void> => {
      try {
        setIsLoading(true);
        setError(null);

        const [habitsData, completionsData, statisticsData] = await Promise.all([
          getHabits(),
          getCompletions(habitId),
          getStatistics(habitId),
        ]);

        const foundHabit = habitsData.find((h) => h.Id === habitId);
        setHabit(foundHabit || null);
        setAllHabits(habitsData);
        setCompletions(completionsData);
        setStatistics(statisticsData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load habit details');
        setHabit(null);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [habitId]);

  if (!habitId) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning">Habit not found.</div>
        <Link to="/" className="btn btn-primary">
          Back to Habits
        </Link>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="container mt-4 d-flex justify-content-center">
        <div className="spinner-border" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger">{error}</div>
        <Link to="/" className="btn btn-primary">
          Back to Habits
        </Link>
      </div>
    );
  }

  if (!habit) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning">Habit not found.</div>
        <Link to="/" className="btn btn-primary">
          Back to Habits
        </Link>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="d-flex align-items-center gap-3 mb-3">
        <Link to="/" className="btn btn-outline-secondary">
          ← Back
        </Link>
        <h2 style={{ fontWeight: 700 }}>{habit.Name}</h2>
      </div>

      {habit.Description && <p style={{ color: '#9ca3af', marginBottom: '1rem' }}>{habit.Description}</p>}

      {/* Calendar Progress Section */}
      <div
        className="tessera-progress-card mt-3"
        style={{
          borderLeft: `5px solid ${habit.Color}`,
        }}
      >
        <h5 className="mb-3">Progress</h5>
        <div style={{ overflowX: 'auto' }}>
          <CalendarGrid
            habitColor={habit.Color}
            completions={completions}
          />
        </div>
      </div>

      {/* Statistics Section */}
      {statistics && (
        <div className="mt-4">
          <StatisticsPanel statistics={statistics} />
        </div>
      )}

      {/* Other Habits Section */}
      {allHabits.length > 0 && (() => {
        const otherHabits = allHabits.filter(h => h.Id !== habitId);
        return otherHabits.length > 0 ? (
          <div className="mt-5">
            <h6 style={{ color: '#9ca3af', textTransform: 'uppercase',
              letterSpacing: '0.5px', marginBottom: '1rem' }}>
              Other Habits
            </h6>
            <div className="d-flex gap-3 flex-wrap">
              {otherHabits.map(h => (
                <div key={h.Id} className="other-habit-chip" onClick={() => navigate(`/habits/${h.Id}`)}
                  style={{
                    background: '#1f2937',
                    border: `2px solid ${h.Color}`,
                    borderRadius: '8px',
                    padding: '0.5rem 1rem',
                    cursor: 'pointer',
                    color: '#f9fafb',
                    fontSize: '0.9rem',
                    fontWeight: 600,
                    transition: 'all 0.15s ease',
                  }}>
                  {h.Name}
                </div>
              ))}
            </div>
          </div>
        ) : null;
      })()}
    </div>
  );
}
