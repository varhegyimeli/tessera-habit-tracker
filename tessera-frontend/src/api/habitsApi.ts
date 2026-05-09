/**
 * API calls for Habit CRUD operations
 * Base URL: https://localhost:7184
 */

import type { Habit } from '../types/index';

const API_BASE = 'https://localhost:7184/api';

/**
 * GET /api/habits - Fetch all habits
 */
export async function getHabits(): Promise<Habit[]> {
  const response = await fetch(`${API_BASE}/habits`);
  if (!response.ok) {
    throw new Error(`Failed to fetch habits: ${response.status}`);
  }
  return response.json();
}

/**
 * POST /api/habits - Create a new habit
 */
export async function createHabit(payload: Omit<Habit, 'Id' | 'CreatedAt'>): Promise<Habit> {
  const response = await fetch(`${API_BASE}/habits`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(`Failed to create habit: ${response.status}`);
  }
  return response.json();
}

/**
 * PUT /api/habits/{habitId} - Update an existing habit
 */
export async function updateHabit(habitId: string, payload: Partial<Habit>): Promise<Habit> {
  const response = await fetch(`${API_BASE}/habits/${habitId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(`Failed to update habit: ${response.status}`);
  }
  return response.json();
}

/**
 * DELETE /api/habits/{habitId} - Delete a habit
 */
export async function deleteHabit(habitId: string): Promise<void> {
  const response = await fetch(`${API_BASE}/habits/${habitId}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    throw new Error(`Failed to delete habit: ${response.status}`);
  }
}
