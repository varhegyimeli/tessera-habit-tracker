import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { CheckInButton } from '../CheckInButton';

describe('CheckInButton', () => {
  it('renders "Check in" button when isCompletedToday is false', () => {
    const onCheckIn = vi.fn();
    const onUndo = vi.fn();
    
    render(
      <CheckInButton
        isCompletedToday={false}
        onCheckIn={onCheckIn}
        onUndo={onUndo}
      />
    );
    
    expect(screen.getByRole('button', { name: /check in/i })).toBeInTheDocument();
  });

  it('renders "Undo" button when isCompletedToday is true', () => {
    const onCheckIn = vi.fn();
    const onUndo = vi.fn();
    
    render(
      <CheckInButton
        isCompletedToday={true}
        onCheckIn={onCheckIn}
        onUndo={onUndo}
      />
    );
    
    expect(screen.getByRole('button', { name: /undo/i })).toBeInTheDocument();
  });

  it('calls onCheckIn when clicked and isCompletedToday is false', async () => {
    const onCheckIn = vi.fn();
    const onUndo = vi.fn();
    const user = userEvent.setup();
    
    render(
      <CheckInButton
        isCompletedToday={false}
        onCheckIn={onCheckIn}
        onUndo={onUndo}
      />
    );
    
    const button = screen.getByRole('button', { name: /check in/i });
    await user.click(button);
    
    expect(onCheckIn).toHaveBeenCalled();
    expect(onUndo).not.toHaveBeenCalled();
  });

  it('calls onUndo when clicked and isCompletedToday is true', async () => {
    const onCheckIn = vi.fn();
    const onUndo = vi.fn();
    const user = userEvent.setup();
    
    render(
      <CheckInButton
        isCompletedToday={true}
        onCheckIn={onCheckIn}
        onUndo={onUndo}
      />
    );
    
    const button = screen.getByRole('button', { name: /undo/i });
    await user.click(button);
    
    expect(onUndo).toHaveBeenCalled();
    expect(onCheckIn).not.toHaveBeenCalled();
  });

  it('button is disabled when isLoading is true', () => {
    const onCheckIn = vi.fn();
    const onUndo = vi.fn();
    
    render(
      <CheckInButton
        isCompletedToday={false}
        onCheckIn={onCheckIn}
        onUndo={onUndo}
        isLoading={true}
      />
    );
    
    const button = screen.getByRole('button');
    expect(button).toBeDisabled();
  });
});
