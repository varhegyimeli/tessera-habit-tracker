/**
 * HabitCard - Shows name, color, and current streak
 * Contains CheckInButton and link to detail view
 */

import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Habit } from '../types/index';
import type { HabitCompletion, HabitStatistics } from '../types/index';
import { getCompletions, createCompletion, deleteCompletion, getStatistics } from '../api/completionsApi';
import { CheckInButton } from './CheckInButton';

interface HabitCardProps {
  habit: Habit;
  onDelete: (id: string) => void;
}

export function HabitCard({ habit, onDelete }: HabitCardProps): React.JSX.Element {
  const navigate = useNavigate();
  const [completions, setCompletions] = useState<HabitCompletion[]>([]);
  const [statistics, setStatistics] = useState<HabitStatistics | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getTodayString = (): string => {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  };

  const todayString = getTodayString();

  // Fetch completions and statistics on mount
  useEffect(() => {
    const fetchData = async (): Promise<void> => {
      try {
        setIsLoading(true);
        setError(null);
        const [completionsData, statisticsData] = await Promise.all([
          getCompletions(habit.Id),
          getStatistics(habit.Id),
        ]);
        setCompletions(completionsData);
        setStatistics(statisticsData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Unknown error');
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [habit.Id]);

  // Determine if completed today and get completion id
  const todayCompletion = completions.find((c) => c.Date === todayString);
  const isCompletedToday = !!todayCompletion;
  const todayCompletionId = todayCompletion?.Id || '';

  // Handle check in
  const handleCheckIn = async (): Promise<void> => {
    try {
      setIsLoading(true);
      setError(null);
      await createCompletion(habit.Id, todayString);
      // Refetch data
      const [completionsData, statisticsData] = await Promise.all([
        getCompletions(habit.Id),
        getStatistics(habit.Id),
      ]);
      setCompletions(completionsData);
      setStatistics(statisticsData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to check in');
    } finally {
      setIsLoading(false);
    }
  };

  // Handle undo
  const handleUndo = async (): Promise<void> => {
    if (!todayCompletionId) return;
    try {
      setIsLoading(true);
      setError(null);
      await deleteCompletion(habit.Id, todayCompletionId);
      // Refetch data
      const [completionsData, statisticsData] = await Promise.all([
        getCompletions(habit.Id),
        getStatistics(habit.Id),
      ]);
      setCompletions(completionsData);
      setStatistics(statisticsData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to undo');
    } finally {
      setIsLoading(false);
    }
  };

  const currentStreak = statistics?.CurrentStreak ?? 0;
  const borderColor = habit.Color;

  return (
    <div
      className="card tessera-card"
      style={{
        borderLeft: `5px solid ${borderColor}`,
        height: '100%',
        cursor: 'pointer',
      }}
      onClick={() => navigate(`/habits/${habit.Id}`)}
    >
      <div className="card-body d-flex flex-column">
        <h5 className="card-title">{habit.Name}</h5>
        <p className="card-text text-muted">🔥 {currentStreak} day streak</p>

        {error && <div className="alert alert-danger small mb-2">{error}</div>}

        <div className="d-flex gap-2 mt-auto" onClick={(e) => e.stopPropagation()}>
          <CheckInButton
            isCompletedToday={isCompletedToday}
            onCheckIn={handleCheckIn}
            onUndo={handleUndo}
            isLoading={isLoading}
          />
          <button
            className="btn btn-outline-danger btn-sm ms-auto"
            onClick={(e) => {
              e.stopPropagation();
              onDelete(habit.Id);
            }}
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
}
