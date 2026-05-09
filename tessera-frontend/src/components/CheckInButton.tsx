/**
 * CheckInButton - Check in or undo for today
 * If today is already completed → "Undo"
 * If not → "Check in"
 */
import React from 'react';

interface CheckInButtonProps {
  isCompletedToday: boolean;
  onCheckIn: () => void;
  onUndo: () => void;
  isLoading?: boolean;
}

export function CheckInButton({
  isCompletedToday,
  onCheckIn,
  onUndo,
  isLoading = false,
}: CheckInButtonProps): React.JSX.Element {
  const buttonClass = isCompletedToday ? 'btn btn-outline-secondary' : 'btn btn-success';
  const buttonText = isCompletedToday ? 'Undo' : 'Check in';
  const handleClick = isCompletedToday ? onUndo : onCheckIn;

  return (
    <button
      className={buttonClass}
      onClick={handleClick}
      disabled={isLoading}
    >
      {buttonText}
    </button>
  );
}
