import { describe, it, expect } from 'vitest';
import { render } from '@testing-library/react';
import type { HabitCompletion } from '../../types/index';
import { CalendarGrid } from '../CalendarGrid';

describe('CalendarGrid', () => {
  it('renders exactly 84 day squares', () => {
    const completions: HabitCompletion[] = [];
    const habitColor = '#34d399';
    
    const { container } = render(
      <CalendarGrid habitColor={habitColor} completions={completions} />
    );
    
    const squares = container.querySelectorAll('div[style*="14px"]');
    expect(squares).toHaveLength(84);
  });

  it('a completed date square has backgroundColor equal to habitColor', () => {
    const today = new Date();
    const todayString = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`;
    
    const completions: HabitCompletion[] = [
      { Id: '1', HabitId: '1', Date: todayString },
    ];
    const habitColor = '#34d399';
    
    const { container } = render(
      <CalendarGrid habitColor={habitColor} completions={completions} />
    );
    
    const todaySquare = container.querySelector(`div[title="${todayString}"]`);
    expect(todaySquare).toHaveStyle(`background-color: ${habitColor}`);
  });

  it('an incomplete date square has backgroundColor #2d3748', () => {
    const today = new Date();
    const yesterdayDate = new Date(today);
    yesterdayDate.setDate(yesterdayDate.getDate() - 1);
    const yesterdayString = `${yesterdayDate.getFullYear()}-${String(yesterdayDate.getMonth() + 1).padStart(2, '0')}-${String(yesterdayDate.getDate()).padStart(2, '0')}`;
    
    const completions: HabitCompletion[] = [];
    const habitColor = '#34d399';
    
    const { container } = render(
      <CalendarGrid habitColor={habitColor} completions={completions} />
    );
    
    const incompleteSquare = container.querySelector(`div[title="${yesterdayString}"]`);
    expect(incompleteSquare).toHaveStyle('background-color: #2d3748');
  });

  it('empty completions array → all squares have backgroundColor #2d3748', () => {
    const completions: HabitCompletion[] = [];
    const habitColor = '#34d399';
    
    const { container } = render(
      <CalendarGrid habitColor={habitColor} completions={completions} />
    );
    
    const squares = container.querySelectorAll('div[style*="14px"]');
    squares.forEach((square) => {
      expect(square).toHaveStyle('background-color: #2d3748');
    });
  });
});
