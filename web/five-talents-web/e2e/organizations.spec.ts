import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';

// Note: the Organizations UI has no delete functionality, so test-created
// orgs are not cleaned up. Names include a timestamp to avoid conflicts
// across runs.

test.describe.serial('Organization flows', () => {
  const ts = Date.now();
  const orgName = `E2e Parish ${ts}`;

  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
  });

  test('create a child organization', async ({ page }) => {
    await page.goto('/organizations');
    await page.getByRole('button', { name: 'Add Organization' }).click();

    await page.getByLabel('Name').fill(orgName);

    // Select the "Campus" level (level 3 — always present in seed data)
    await page.locator('mat-select[formcontrolname="level"]').click();
    await page.waitForSelector('mat-option', { timeout: 5_000 });
    await page.getByRole('option', { name: 'Campus' }).click();

    await page.getByRole('button', { name: 'Create' }).click();

    // Verify name appears in the tree
    await expect(page.locator('.org-name', { hasText: orgName })).toBeVisible();
  });

  test('edit organization settings', async ({ page }) => {
    await page.goto('/organizations');

    // Find the org's settings button in the tree view
    const row = page.locator('.tree-row', { hasText: orgName });
    await row.locator('button').filter({ has: page.locator('mat-icon', { hasText: 'settings' }) }).click();

    // Change currency to EUR
    await page.getByLabel('Currency').click();
    await page.getByRole('option', { name: /EUR/i }).click();

    await page.getByRole('button', { name: 'Save Settings' }).click();

    // Dialog should close
    await expect(page.locator('mat-dialog-container')).not.toBeVisible();
  });
});
