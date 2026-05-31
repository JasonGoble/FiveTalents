# ADR-0016: Testing Strategy — Unit, Integration, E2E, and CI

**Status:** Accepted
**Date:** 2026-05-30

## Context

The project reached a stable feature set covering Members, Families, Groups, Organizations, and User Accounts. Before adding further features, a testing foundation was needed to catch regressions, validate the request pipeline, and give contributors confidence when changing shared infrastructure.

The main forces:

- The .NET backend follows Clean Architecture with CQRS handlers as the primary unit of business logic — the natural seam to test at
- The API uses `WebApplicationFactory` for integration tests so the full middleware/auth/validation pipeline can be exercised without a real HTTP server
- Angular 21 ships `@angular/build:unit-test` with Vitest support out of the box
- E2E tests must run against the real full stack to catch integration gaps not visible at lower levels
- CI must be fast enough to be useful on every PR while still providing meaningful signal

## Decision

### .NET unit tests (`FiveTalents.Tests.Unit`)

- **Framework:** xUnit + FluentAssertions + NSubstitute
- **Target:** MediatR command/query handlers in `FiveTalents.Application`
- **Isolation:** EF Core `InMemory` provider, one fresh `DbContext` per test class; no HTTP layer
- **What is skipped:** Identity and email handlers (`InviteMember`, `LinkUser`, `UnlinkUser`) are excluded from unit tests because they wrap `UserManager<T>` and `IEmailService` — both are difficult to mock meaningfully. These paths are covered by integration tests instead.

### .NET integration tests (`FiveTalents.Tests.Integration`)

- **Framework:** xUnit + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`)
- **Database:** SQLite `:memory:` via `IntegrationTestFactory`, one shared connection per factory instance. The factory sets `ASPNETCORE_ENVIRONMENT=Testing` (via `builder.UseEnvironment("Testing")`), which causes the app to load `appsettings.Testing.json` (`DatabaseProvider=Sqlite`) before any DI registration runs. This prevents the dual-provider conflict that would occur if Npgsql were registered first and then overridden.
- **Scope:** Full HTTP request pipeline — routing, middleware, FluentValidation, auth, EF Core
- **Coverage:** All controller endpoints have at least one test; auth-gated endpoints are tested for both 401 (unauthenticated) and 2xx (authenticated) responses

### Angular unit tests

- **Runner:** Vitest via `@angular/build:unit-test` (`runner: "vitest"` in `angular.json`)
- **Environment:** Node.js + jsdom (no real browser needed)
- **Libraries:** `@testing-library/angular` and `@testing-library/user-event` for component tests
- **Coverage:** All 7 Angular services (via `HttpTestingController`), `authGuard`, `EntityAutocompleteComponent` (ControlValueAccessor behaviour), `DateInputDirective` (keyboard/paste formatting)

### E2E tests

- **Tool:** Playwright (`@playwright/test`)
- **Scope:** Critical happy-path flows — auth (login, logout), member CRUD, family creation and member assignment
- **Stack:** Both servers started via Playwright's `webServer` config; `reuseExistingServer: !isCI` so local dev reuses running servers and CI starts fresh
- **Browser:** Chromium. On Ubuntu 26.04+ (where Playwright's bundled headless shell is unsupported) the config auto-detects `/usr/bin/chromium-browser` and drives it with `--headless` via launch args; on CI (Ubuntu 22.04) Playwright installs its own binary normally

### Code coverage

- **Tool:** Coverlet (`coverlet.collector`) on both test projects; output format `opencover`
- **Report:** `reportgenerator` (local tool in `.config/dotnet-tools.json`) generates an HTML report to `coverage/html/`
- **Threshold:** Enforced in CI at **70% line coverage**. PostgreSQL migration files (`FiveTalents.Infrastructure/Migrations/*.cs`) are excluded from coverage via `coverlet.runsettings` — they are auto-generated EF Core scaffolding, not business logic. With that exclusion, current line coverage is ~86%

### CI (GitHub Actions)

Three jobs run on every push to `main` and every non-draft PR:

1. **.NET Tests & Coverage** — `dotnet build` → `dotnet test --settings coverlet.runsettings` → `reportgenerator` JSON summary → threshold check. HTML report uploaded as a 14-day artifact.
2. **Angular Tests** — `npm ci` → `npm test` (Vitest, single run, exits)
3. **E2E Tests** — skipped on draft PRs; creates `appsettings.Development.json` for SQLite, installs Playwright Chromium, runs `npm run e2e`, uploads `playwright-report/` on failure

Format compliance (`dotnet format --verify-no-changes`) is enforced as the first step of the pre-commit checklist in `CLAUDE.md` but is not yet a CI gate (see Consequences).

## Consequences

**Easier:**
- Regressions in handler logic are caught immediately by unit tests with fast feedback
- The full request pipeline — routing, auth, validation, EF Core queries — is validated by integration tests without a running server or real database
- Angular service and component contracts are verified without a browser
- E2E tests give confidence that the UI and API interact correctly for the most critical flows
- CI blocks merges when any test suite fails or coverage drops below the floor

**Harder:**
- Two migration assemblies already exist (SQLite + Postgres); the `appsettings.Testing.json` SQLite override is an additional configuration surface that must stay in sync with the test factory if the provider selection logic ever changes
- Identity/email flows (invite, link/unlink user) have no dedicated unit tests — they rely on integration tests, which are slower and share a global in-memory database per factory instance
- The 70% coverage target requires continued investment in test authorship; until reached, the CI threshold is intentionally low to avoid permanently red CI
- `dotnet format --verify-no-changes` is not yet enforced in CI — a backlog of pre-existing formatting violations must be resolved before this gate can be added without breaking every PR
