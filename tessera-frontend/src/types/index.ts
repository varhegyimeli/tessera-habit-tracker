/**
 * Domain types for Tessera habit tracker
 */

export interface Habit {
  Id: string;          // Guid
  Name: string;
  Description?: string;
  Color: string;       // hex, e.g. "#4CAF50"
  CreatedAt: string;   // ISO 8601
}

export interface HabitCompletion {
  Id: string;          // Guid
  HabitId: string;
  Date: string;        // "YYYY-MM-DD" (DateOnly)
}

export interface HabitStatistics {
  CurrentStreak: number;
  LongestStreak: number;
  CompletionRate: number;  // 0.0–1.0, last 30 days
}
