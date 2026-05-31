import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';

test.describe.serial('Group CRUD', () => {
  const ts = Date.now();
  const groupName = `E2e Group ${ts}`;
  const editedName = `E2e Group ${ts} Edited`;

  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
  });

  test('create a group', async ({ page }) => {
    await page.goto('/groups');
    await page.getByRole('button', { name: 'New Group' }).click();

    await page.getByLabel('Group Name').fill(groupName);

    // Select the first available group type
    await page.getByLabel('Group Type').click();
    await page.getByRole('option').first().click();

    await page.getByRole('button', { name: 'Create Group' }).click();

    await expect(page.locator('.group-card', { hasText: groupName })).toBeVisible();
  });

  test('edit a group', async ({ page }) => {
    await page.goto('/groups');
    const card = page.locator('.group-card', { hasText: groupName });
    await card.locator('button').filter({ has: page.locator('mat-icon', { hasText: 'edit' }) }).click();

    await page.getByLabel('Group Name').clear();
    await page.getByLabel('Group Name').fill(editedName);
    await page.getByRole('button', { name: 'Save Changes' }).click();

    await expect(page.locator('.group-card', { hasText: editedName })).toBeVisible();
  });

  test('delete a group', async ({ page }) => {
    await page.goto('/groups');
    const card = page.locator('.group-card', { hasText: editedName });
    page.once('dialog', dialog => dialog.accept());
    await card.locator('button').filter({ has: page.locator('mat-icon', { hasText: 'delete_outline' }) }).click();

    await expect(page.locator('.group-card', { hasText: editedName })).not.toBeVisible();
  });
});
