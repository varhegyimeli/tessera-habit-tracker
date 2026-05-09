/**
 * HomePage - Main page showing all habits
 * Displays HabitList component
 */

import React from 'react';
import { Link } from 'react-router-dom';
import { HabitList } from '../components/HabitList';

export function HomePage(): React.JSX.Element {
  return (
    <>
      <nav className="tessera-navbar">
        <div className="container">
          <div className="d-flex align-items-center">
            <Link to="/" style={{ textDecoration: 'none' }}>
              <img
                src="/tessera-logo.png"
                alt="Tessera logo"
                style={{ height: '88px', width: '88px', objectFit: 'contain', marginRight: '0.75rem' }}
              />
            </Link>
            <div>
              <Link to="/" style={{ textDecoration: 'none', color: 'inherit' }}>
                <h1>Tessera</h1>
              </Link>
              <p className="tagline">Your habits, one tile at a time.</p>
            </div>
          </div>
        </div>
      </nav>
      <div className="container mt-4">
        <h1 className="mb-3">My Habits</h1>
        <HabitList habits={[]} />
      </div>
    </>
  );
}
