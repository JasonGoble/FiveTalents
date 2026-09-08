import { defineConfig } from '@playwright/test';
import { existsSync } from 'fs';

const isCI = !!process.env['CI'];
const systemChromium = '/usr/bin/chromium-browser';

// Set E2E_BASE_URL to run this suite against an already-running stack
// (e.g. the docker-compose stack on a remote Docker host) instead of
// spawning native `dotnet run`/`npm start` against localhost.
const remoteBaseUrl = process.env['E2E_BASE_URL'];

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
  // A remote target adds real network latency, which flushes out timing
  // assumptions (e.g. Material dropdown opens) that concurrent spec files
  // don't hit against localhost. Serialize to keep this deterministic.
  workers: remoteBaseUrl ? 1 : undefined,
  timeout: 30_000,
  reporter: [['html', { open: 'never', outputFolder: 'playwright-report' }]],
  outputDir: 'test-results',
  use: {
    baseURL: remoteBaseUrl ?? 'http://localhost:4200',
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
  webServer: remoteBaseUrl
    ? undefined
    : [
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
