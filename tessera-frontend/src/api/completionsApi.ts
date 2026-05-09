/**
 * API calls for HabitCompletion and Statistics
 * Base URL: https://localhost:7184
 */

import type { HabitCompletion, HabitStatistics } from '../types/index';

const API_BASE = 'https://localhost:7184/api';

/**
 * GET /api/habits/{habitId}/completions - Fetch all completions for a habit
 */
export async function getCompletions(habitId: string): Promise<HabitCompletion[]> {
  const response = await fetch(`${API_BASE}/habits/${habitId}/completions`);
  if (!response.ok) {
    throw new Error(`Failed to fetch completions: ${response.status}`);
  }
  return response.json();
}

/**
 * POST /api/habits/{habitId}/completions - Create a new completion (check in)
 */
export async function createCompletion(habitId: string, date: string): Promise<HabitCompletion> {
  const response = await fetch(`${API_BASE}/habits/${habitId}/completions`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ Date: date }),
  });
  if (!response.ok) {
    throw new Error(`Failed to create completion: ${response.status}`);
  }
  return response.json();
}

/**
 * DELETE /api/habits/{habitId}/completions/{completionId} - Delete a completion (undo)
 */
export async function deleteCompletion(habitId: string, completionId: string): Promise<void> {
  const response = await fetch(
    `${API_BASE}/habits/${habitId}/completions/${completionId}`,
    {
      method: 'DELETE',
    }
  );
  if (!response.ok) {
    throw new Error(`Failed to delete completion: ${response.status}`);
  }
}

/**
 * GET /api/habits/{habitId}/statistics - Fetch statistics for a habit
 */
export async function getStatistics(habitId: string): Promise<HabitStatistics> {
  const response = await fetch(`${API_BASE}/habits/${habitId}/statistics`);
  if (!response.ok) {
    throw new Error(`Failed to fetch statistics: ${response.status}`);
  }
  return response.json();
}
