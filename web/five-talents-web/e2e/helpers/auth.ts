import { Page } from '@playwright/test';

export const ADMIN_EMAIL = 'admin@FiveTalents.local';
export const ADMIN_PASSWORD = 'Admin1234!';

export async function loginAsAdmin(page: Page): Promise<void> {
  await page.goto('/auth/login');
  await page.fill('#email', ADMIN_EMAIL);
  await page.fill('#password', ADMIN_PASSWORD);
  await page.click('button[type="submit"]');
  await page.waitForURL('**/dashboard');
}
