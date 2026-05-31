import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';

test('profile page loads for authenticated user', async ({ page }) => {
  await loginAsAdmin(page);
  await page.goto('/profile');

  // The seed admin account has no linked member record, so the "not linked"
  // state is expected. The test verifies the page renders without error.
  await expect(page.locator('h1')).toContainText('My Profile');
  await expect(
    page.locator('mat-card').filter({ hasText: /not linked|Personal Information/i })
  ).toBeVisible();
});
