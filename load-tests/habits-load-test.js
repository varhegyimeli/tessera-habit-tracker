// k6 Load Test for Tessera.Api
// Requires Tessera.Api running on https://localhost:7184
// Start with Ctrl+F5 in Visual Studio before running
// Run with: k6 run load-tests/habits-load-test.js

import http from 'k6/http';
import { check, sleep } from 'k6';

// Load test configuration
export const options = {
  stages: [
    // Ramp-up: 0 ? 10 virtual users over 10 seconds
    { duration: '10s', target: 10 },
    // Steady: 10 virtual users for 30 seconds
    { duration: '30s', target: 10 },
    // Ramp-down: 10 ? 0 virtual users over 10 seconds
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    // 95% of requests must complete under 500ms
    'http_req_duration{staticAsset:no}': ['p(95)<500'],
    // Less than 1% of requests should fail
    'http_req_failed': ['rate<0.01'],
  },
};

const BASE_URL = 'https://localhost:7184';
const HTTP_OPTIONS = {
  insecureSkipTLSVerify: true, // Development certificate
};

/**
 * Fetches all habits and checks the response
 * @returns {Object|null} The first habit or null if list is empty
 */
function getHabits() {
  const url = `${BASE_URL}/api/habits`;
  const res = http.get(url, HTTP_OPTIONS);

  // Check response status
  check(res, {
    'GET /api/habits - status is 200': (r) => r.status === 200,
    'GET /api/habits - response time < 500ms': (r) => r.timings.duration < 500,
  });

  // Parse and return first habit if available
  try {
    const habits = JSON.parse(res.body);
    if (Array.isArray(habits) && habits.length > 0) {
      return habits[0]; // Return first habit
    }
  } catch (e) {
    console.error(`Failed to parse habits response: ${e}`);
  }

  return null;
}

/**
 * Fetches statistics for a specific habit
 * @param {string} habitId The habit ID
 */
function getHabitStatistics(habitId) {
  const url = `${BASE_URL}/api/habits/${habitId}/statistics`;
  const res = http.get(url, HTTP_OPTIONS);

  // Check response status and content
  check(res, {
    'GET /api/habits/{id}/statistics - status is 200': (r) => r.status === 200,
    'GET /api/habits/{id}/statistics - response time < 500ms': (r) => r.timings.duration < 500,
    'GET /api/habits/{id}/statistics - contains CurrentStreak': (r) => r.body.includes('CurrentStreak'),
  });
}

/**
 * Main test scenario - runs for each virtual user
 */
export default function () {
  // Step 1: Get all habits
  const habit = getHabits();

  // Step 2: If habits exist, get statistics for the first one
  if (habit && habit.Id) {
    getHabitStatistics(habit.Id);
  }

  // Step 3: Think time - 1 second between iterations
  sleep(1);
}