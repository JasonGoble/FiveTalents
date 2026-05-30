import { test, expect } from '@playwright/test';
import { loginAsAdmin } from './helpers/auth';

test.describe.serial('Family flows', () => {
  const ts = Date.now();
  const familyName = `E2e Family ${ts}`;
  const memberFirst = 'E2e';
  const memberLast = `FamilyMember${ts}`;
  const memberFullName = `${memberFirst} ${memberLast}`;
  let memberUrl = '';

  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
  });

  test('setup - create test member', async ({ page }) => {
    await page.goto('/members/new');
    await page.getByLabel('First Name').fill(memberFirst);
    await page.getByLabel('Last Name').fill(memberLast);
    await page.getByRole('button', { name: 'Add Member' }).click();
    await page.waitForURL(/\/members\/\d+$/);
    memberUrl = page.url();
  });

  test('create a family', async ({ page }) => {
    await page.goto('/families');
    await page.getByRole('button', { name: 'Add Family' }).click();
    await page.getByLabel('Family Name').fill(familyName);
    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.getByText(familyName)).toBeVisible();
  });

  test('add a member to the family', async ({ page }) => {
    await page.goto('/families');
    await page.locator('a', { hasText: familyName }).click();
    await page.waitForURL(/\/families\/\d+$/);

    await page.getByRole('button', { name: 'Add Member' }).click();
    await page.getByRole('combobox', { name: 'Member' }).fill(memberLast);
    await page.waitForSelector('[role="option"]', { timeout: 10_000 });
    await page.getByRole('option', { name: memberFullName }).click();
    await page.getByRole('button', { name: 'Add' }).click();

    await expect(page.getByText(memberFullName)).toBeVisible();
  });

  test('cleanup - delete family', async ({ page }) => {
    await page.goto('/families');
    await page.locator('a', { hasText: familyName }).click();
    await page.waitForURL(/\/families\/\d+$/);

    page.once('dialog', dialog => dialog.accept());
    await page.getByRole('button', { name: 'Delete' }).click();
    await page.waitForURL(/\/families$/);
    await expect(page.getByText(familyName)).not.toBeVisible();
  });

  test('cleanup - delete test member', async ({ page }) => {
    await page.goto(memberUrl);
    page.once('dialog', dialog => dialog.accept());
    await page.getByRole('button', { name: 'Delete' }).click();
    await page.waitForURL(/\/members$/);
  });
});
