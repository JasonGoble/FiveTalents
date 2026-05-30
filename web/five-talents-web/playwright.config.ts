import { defineConfig } from '@playwright/test';
import { existsSync } from 'fs';

// On Ubuntu 26.04+ Playwright's bundled Chromium isn't supported; use the system binary when present.
// Set CHROMIUM_PATH to override (e.g. in CI where a supported OS is used and the binary is installed).
const systemChromium = '/usr/bin/chromium-browser';
const executablePath =
  process.env['CHROMIUM_PATH'] ??
  (existsSync(systemChromium) ? systemChromium : undefined);

export default defineConfig({
  testDir: './e2e',
  fullyParallel: false,
  forbidOnly: !!process.env['CI'],
  retries: process.env['CI'] ? 1 : 0,
  timeout: 30_000,
  reporter: [['html', { open: 'never', outputFolder: 'playwright-report' }]],
  outputDir: 'test-results',
  use: {
    baseURL: 'http://localhost:4200',
    screenshot: 'only-on-failure',
    // headless: false here so Playwright uses the full Chromium binary (not the headless shell).
    // --headless=new is passed as a launch arg so it still runs without a display.
    headless: false,
    launchOptions: {
      args: ['--headless', '--no-sandbox', '--disable-dev-shm-usage'],
      ...(executablePath ? { executablePath } : {}),
    },
  },
  projects: [
    { name: 'chromium', use: { browserName: 'chromium' } },
  ],
  webServer: [
    {
      command: 'dotnet run --project ../../src/FiveTalents.Api',
      url: 'http://localhost:5290/openapi/v1.json',
      reuseExistingServer: true,
      timeout: 60_000,
    },
    {
      command: 'npm start',
      url: 'http://localhost:4200',
      reuseExistingServer: true,
      timeout: 120_000,
    },
  ],
});
