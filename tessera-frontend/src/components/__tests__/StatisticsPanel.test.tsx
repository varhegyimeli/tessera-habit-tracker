import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { StatisticsPanel } from '../StatisticsPanel';

describe('StatisticsPanel', () => {
  it('renders CurrentStreak value with "days" label', () => {
    const statistics = {
      CurrentStreak: 5,
      LongestStreak: 10,
      CompletionRate: 0.75,
    };
    
    render(<StatisticsPanel statistics={statistics} />);
    
    expect(screen.getByText('5 days')).toBeInTheDocument();
    expect(screen.getByText(/current streak/i)).toBeInTheDocument();
  });

  it('renders LongestStreak value with "days" label', () => {
    const statistics = {
      CurrentStreak: 5,
      LongestStreak: 20,
      CompletionRate: 0.75,
    };
    
    render(<StatisticsPanel statistics={statistics} />);
    
    expect(screen.getByText('20 days')).toBeInTheDocument();
    expect(screen.getByText(/longest streak/i)).toBeInTheDocument();
  });

  it('renders CompletionRate as percentage (e.g. 0.75 → "75%")', () => {
    const statistics = {
      CurrentStreak: 5,
      LongestStreak: 10,
      CompletionRate: 0.75,
    };
    
    render(<StatisticsPanel statistics={statistics} />);
    
    expect(screen.getByText('75%')).toBeInTheDocument();
    expect(screen.getByText(/completion rate/i)).toBeInTheDocument();
  });

  it('renders "0%" when CompletionRate is 0', () => {
    const statistics = {
      CurrentStreak: 0,
      LongestStreak: 0,
      CompletionRate: 0,
    };
    
    render(<StatisticsPanel statistics={statistics} />);
    
    expect(screen.getByText('0%')).toBeInTheDocument();
  });
});
