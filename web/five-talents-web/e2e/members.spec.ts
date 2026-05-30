import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';

test.describe.serial('Member CRUD', () => {
  const ts = Date.now();
  const firstName = 'E2e';
  const lastName = `Member${ts}`;
  const fullName = `${firstName} ${lastName}`;
  let memberUrl = '';

  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
  });

  test('create a member', async ({ page }) => {
    await page.goto('/members/new');
    await page.getByLabel('First Name').fill(firstName);
    await page.getByLabel('Last Name').fill(lastName);
    await page.getByRole('button', { name: 'Add Member' }).click();
    await page.waitForURL(/\/members\/\d+$/);
    memberUrl = page.url();
    await expect(page.locator('h1.member-name')).toContainText(fullName);
  });

  test('view member detail shows personal information', async ({ page }) => {
    await page.goto(memberUrl);
    await expect(page.locator('h1.member-name')).toContainText(fullName);
    await expect(page.getByText('Personal Information')).toBeVisible();
  });

  test('edit a member', async ({ page }) => {
    await page.goto(`${memberUrl}/edit`);
    await page.getByLabel('Last Name').waitFor();
    await page.getByLabel('Last Name').clear();
    await page.getByLabel('Last Name').fill(`${lastName}Edited`);
    await page.getByRole('button', { name: 'Save Changes' }).click();
    await page.waitForURL(/\/members\/\d+$/);
    await expect(page.locator('h1.member-name')).toContainText(`${firstName} ${lastName}Edited`);
  });

  test('delete a member', async ({ page }) => {
    await page.goto(memberUrl);
    page.once('dialog', dialog => dialog.accept());
    await page.getByRole('button', { name: 'Delete' }).click();
    await page.waitForURL(/\/members$/);
    await page.getByPlaceholder('Name or email...').fill(`${lastName}Edited`);
    await page.waitForTimeout(600);
    await expect(page.locator('tbody tr, .member-card').filter({ hasText: `${firstName} ${lastName}Edited` })).toHaveCount(0);
  });
});
