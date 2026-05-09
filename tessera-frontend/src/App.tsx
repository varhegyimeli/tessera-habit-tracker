/**
 * App - Main application component with routing
 * Routes:
 * - / → HomePage
 * - /habits/:habitId → HabitDetailPage
 */

import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { HomePage } from './pages/HomePage';
import { HabitDetailPage } from './pages/HabitDetailPage';

function App(): React.JSX.Element {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/habits/:habitId" element={<HabitDetailPage />} />
      </Routes>
    </Router>
  );
}

export default App;
