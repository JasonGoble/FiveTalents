import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';

test('invalid login stays on login page with 401 response', async ({ page }) => {
  await page.goto('/auth/login');
  await page.fill('#email', 'nobody@example.com');
  await page.fill('#password', 'wrongpassword');
  const responsePromise = page.waitForResponse('**/api/auth/login');
  await page.click('button[type="submit"]');
  const response = await responsePromise;
  expect(response.status()).toBe(401);
  await page.waitForTimeout(500);
  await expect(page).toHaveURL(/\/auth\/login/);
});

test('valid login redirects to dashboard', async ({ page }) => {
  await loginAsAdmin(page);
  await expect(page).toHaveURL(/\/dashboard/);
});

test('logout clears session and redirects to login', async ({ page }) => {
  await loginAsAdmin(page);
  await page.locator('.user-info button').click();
  await expect(page).toHaveURL(/\/auth\/login/);
});
