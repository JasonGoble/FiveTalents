import { defineConfig } from '@playwright/test';
import { existsSync } from 'fs';

const isCI = !!process.env['CI'];
const systemChromium = '/usr/bin/chromium-browser';

// On Ubuntu 26.04+, Playwright's bundled headless shell isn't supported.
// Locally, fall back to the system Chromium and drive it headless via --headless arg.
// In CI (Ubuntu 22.04/24.04), Playwright installs its own binary — no override needed.
const useSystemChromium = !isCI && (!!process.env['CHROMIUM_PATH'] || existsSync(systemChromium));
const executablePath = useSystemChromium
  ? (process.env['CHROMIUM_PATH'] ?? systemChromium)
  : undefined;

export default defineConfig({
  testDir: './e2e',
  fullyParallel: false,
  forbidOnly: isCI,
  retries: isCI ? 1 : 0,
  timeout: 30_000,
  reporter: [['html', { open: 'never', outputFolder: 'playwright-report' }]],
  outputDir: 'test-results',
  use: {
    baseURL: 'http://localhost:4200',
    screenshot: 'only-on-failure',
    // In CI, use standard headless mode (headless shell or Playwright's default).
    // Locally on Ubuntu 26.04+, headless: false avoids the headless shell; --headless arg
    // keeps it display-free while using the full Chromium binary.
    headless: !useSystemChromium,
    launchOptions: {
      args: [
        '--no-sandbox',
        '--disable-dev-shm-usage',
        ...(useSystemChromium ? ['--headless'] : []),
      ],
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
      reuseExistingServer: !isCI,
      timeout: 60_000,
    },
    {
      command: 'npm start',
      url: 'http://localhost:4200',
      reuseExistingServer: !isCI,
      timeout: 180_000,
    },
  ],
});
