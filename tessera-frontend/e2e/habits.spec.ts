// Requires: npm run dev (Vite on port 5173) + Tessera.Api running on port 7184

import { test, expect } from '@playwright/test';

test.describe('Habit management', () => {
  test('homepage loads and shows the app title', async ({ page }) => {
    await page.goto('/');
    
    await expect(page.locator('nav')).toBeVisible({ timeout: 10000 });
  });

  test('can open the new habit form', async ({ page }) => {
    await page.goto('/');
    
    await page.getByRole('button', { name: /new habit/i }).click();
    
    const input = page.getByPlaceholder('e.g., Morning Exercise');
    await expect(input).toBeVisible();
  });

  test('can create a new habit', async ({ page }) => {
    await page.goto('/');
    
    await page.getByRole('button', { name: /new habit/i }).click();
    
    const habitName = `Test Habit ${Date.now()}`;
    await page.getByPlaceholder('e.g., Morning Exercise').fill(habitName);
    
    await page.getByRole('button', { name: /add habit/i }).click();
    
    // Wait for the habit to appear on the page
    await expect(page.locator('body')).toContainText(habitName, { timeout: 10000 });
  });

  test('can check in a habit', async ({ page }) => {
    await page.goto('/');
    
    // Wait for habit cards to load
    await page.getByRole('button', { name: /check in/i }).first().waitFor({ state: 'visible', timeout: 10000 });
    
    // Click the first "Check in" button
    await page.getByRole('button', { name: /check in/i }).first().click();
    
    // Assert "Undo" button appears
    await expect(page.getByRole('button', { name: /undo/i }).first()).toBeVisible();
  });

  test('can navigate to habit detail page', async ({ page }) => {
    await page.goto('/');
    
    // Wait for at least one habit card to be visible
    const habitCard = page.locator('[class*="tessera-card"]').first();
    await habitCard.waitFor({ state: 'visible', timeout: 10000 });
    
    // Click on the first habit card
    await habitCard.click();
    
    // Assert URL contains '/habits/'
    await expect(page).toHaveURL(/\/habits\/.+/, { timeout: 10000 });
    
    // Assert page contains 'Current Streak' text (StatisticsPanel is rendered)
    await expect(page.locator('body')).toContainText('Current Streak', { timeout: 10000 });
  });
});
