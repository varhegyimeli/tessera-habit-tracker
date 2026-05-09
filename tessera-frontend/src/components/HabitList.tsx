/**
 * HabitList - Renders all habits
 * One HabitCard per habit
 */

import React, { useEffect, useState } from 'react';
import type { Habit } from '../types/index';
import { getHabits, createHabit, deleteHabit } from '../api/habitsApi';
import { HabitCard } from './HabitCard';

interface HabitListProps {
  habits: Habit[];
}

export function HabitList({ habits: initialHabits }: HabitListProps): React.JSX.Element {
  const [habits, setHabits] = useState<Habit[]>(initialHabits);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    color: '#4CAF50',
  });

  // Fetch habits on mount
  useEffect(() => {
    const fetchHabits = async (): Promise<void> => {
      try {
        setIsLoading(true);
        setError(null);
        const data = await getHabits();
        setHabits(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch habits');
      } finally {
        setIsLoading(false);
      }
    };

    fetchHabits();
  }, []);

  // Handle form input changes
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>): void => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  // Handle form submission
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();
    if (!formData.name.trim()) {
      setError('Habit name is required');
      return;
    }

    try {
      setIsLoading(true);
      setError(null);
      await createHabit({
        Name: formData.name,
        Description: formData.description || undefined,
        Color: formData.color,
      });
      // Refetch habits
      const data = await getHabits();
      setHabits(data);
      // Reset form
      setFormData({
        name: '',
        description: '',
        color: '#4CAF50',
      });
      setShowForm(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create habit');
    } finally {
      setIsLoading(false);
    }
  };

  // Handle delete
  const handleDelete = async (habitId: string): Promise<void> => {
    try {
      setIsLoading(true);
      setError(null);
      await deleteHabit(habitId);
      // Refetch habits
      const data = await getHabits();
      setHabits(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete habit');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <button
          className="btn btn-primary"
          onClick={() => setShowForm(!showForm)}
        >
          {showForm ? 'Cancel' : 'New Habit'}
        </button>
      </div>

      {/* Add Habit Form */}
      {showForm && (
        <div className="card mb-4">
          <div className="card-body">
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label htmlFor="name" className="form-label">
                  Habit Name *
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  placeholder="e.g., Morning Exercise"
                  required
                />
              </div>

              <div className="mb-3">
                <label htmlFor="description" className="form-label">
                  Description
                </label>
                <textarea
                  className="form-control"
                  id="description"
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  placeholder="Optional description"
                  rows={3}
                />
              </div>

              <div className="mb-3">
                <label htmlFor="color" className="form-label">
                  Color
                </label>
                <input
                  type="color"
                  className="form-control form-control-color"
                  id="color"
                  name="color"
                  value={formData.color}
                  onChange={handleInputChange}
                />
              </div>

              <div className="d-flex gap-2">
                <button type="submit" className="btn btn-success">
                  Add Habit
                </button>
                <button
                  type="button"
                  className="btn btn-outline-secondary"
                  onClick={() => {
                    setShowForm(false);
                    setFormData({
                      name: '',
                      description: '',
                      color: '#4CAF50',
                    });
                  }}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Error Alert */}
      {error && <div className="alert alert-danger">{error}</div>}

      {/* Loading Spinner */}
      {isLoading && (
        <div className="d-flex justify-content-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      )}

      {/* Habits Grid */}
      {!isLoading && habits.length === 0 && !showForm && (
        <div className="alert alert-info">No habits yet. Create one to get started!</div>
      )}

      <div className="row g-3">
        {habits.map((habit) => (
          <div key={habit.Id} className="col-md-6 col-lg-4">
            <HabitCard habit={habit} onDelete={handleDelete} />
          </div>
        ))}
      </div>
    </>
  );
}
