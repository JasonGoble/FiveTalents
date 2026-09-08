# FiveTalents — Claude Working Guide

Project conventions and technical gotchas for AI-assisted development on this repo.

## Tech Stack (quick reference)

- **API:** .NET 10, ASP.NET Core, MediatR (CQRS), FluentValidation, EF Core 10 + SQLite (dev) / PostgreSQL (prod)
- **Auth:** ASP.NET Core Identity + JWT Bearer; enums serialized as **strings** via `JsonStringEnumConverter`
- **Frontend:** Angular 21, standalone components, Angular Material, signals-first state
- **Architecture:** Clean Architecture — Domain → Application → Infrastructure → Api

## Running Locally

Local dev requires **no external services** — SQLite is the default database and dev emails are written as `.eml` files to `logs/emails/`. Press F5 in VS Code ("Full Stack") to build the API and launch Angular with debuggers attached.

```bash
# Migrations (dotnet-ef global tool)
# SQLite (default dev):
dotnet-ef migrations add <Name> --project src/FiveTalents.Migrations.Sqlite --startup-project src/FiveTalents.Api
dotnet-ef database update --project src/FiveTalents.Migrations.Sqlite --startup-project src/FiveTalents.Api

# PostgreSQL (production / opt-in local):
dotnet-ef migrations add <Name> --project src/FiveTalents.Infrastructure --startup-project src/FiveTalents.Api
dotnet-ef database update --project src/FiveTalents.Infrastructure --startup-project src/FiveTalents.Api

# API
dotnet run --project src/FiveTalents.Api   # http://localhost:5290

# Frontend
cd web/five-talents-web && npm start       # http://localhost:4200
```

> **Note:** `appsettings.Development.json` is gitignored. It defaults to SQLite (`DatabaseProvider: Sqlite`, `Data Source=FiveTalents.db`). Override with `DatabaseProvider: Postgres` and a connection string to use PostgreSQL locally.
> Docker (`docker-compose.yml`) isn't used for day-to-day local dev (native `dotnet run` + SQLite is faster to iterate on), but it **is** the prod-parity check before a PR — see [E2E Tests (Playwright)](#e2e-tests-playwright). Local dev's SQLite + native process model diverges from what's actually deployed (real Postgres, containerized packaging), and that gap has caused real bugs invisible to local dev alone.

## Branching & GitHub Workflow

- **GitHub Flow:** `feature/<issue#>-<slug>` or `fix/<issue#>-<slug>` → PR → merge to `main`
- Direct pushes to `main` are blocked; JasonGoble is on the bypass list
- Assign `JasonGoble` to an issue when work **begins** on it (not before)
- Include `Fixes #N` or `Closes #N` in commit messages so GitHub auto-closes issues on merge

## GitHub Issue Conventions

Title prefix and body structure below match `.github/ISSUE_TEMPLATE/feature_request.md` and `bug_report.md` — use `gh issue create --template` where possible, or match those sections by hand rather than freelancing a different header set.

**Title format:** `feat:`, `bug:`, or `chore:` prefix — lowercase, e.g. `feat: attendance tracking`

**Labels:**
| Issue type | Labels |
|------------|--------|
| `feat:` | `feature`, `backend`, `frontend` |
| `bug:` | `bug` |
| `chore:` | `chore` |

Every issue — regardless of type — must also have at least one `area:` label (e.g. `area:members`, `area:families`, `area:groups`). Apply this when creating or triaging issues.

**Bug issue body structure:** Symptom → Root Cause → Fix → Fixed In (commit SHA)

Create a GitHub issue for **every** reported defect, even small ones. The value is the cumulative searchable record linking symptoms to commits.

## Before Every Commit

### Step 1 — Format & build

For any .NET changes, run in order:

```bash
# 1. Auto-fix editorconfig violations, then verify clean
dotnet format
dotnet format --verify-no-changes

# 2. Compile
dotnet build

# 3. Tests (if test project is affected or handler/domain code changed)
dotnet test
```

For Angular changes: `npm run build` from `web/five-talents-web`.

Fix any format, build, or test failures before moving to step 2.

### Step 2 — E2E tests, against both targets (when the API or web app changed)

If the change touches any backend project (`FiveTalents.Api`, `.Application`, `.Domain`, `.Infrastructure`) or the Angular app (`web/five-talents-web`), run the Playwright E2E suite against **both** of the following before proposing a commit:

1. **Local dev target** — native `dotnet run` + `npm start`, SQLite. The default `npm run e2e` flow.
2. **Docker/Postgres target** — the containerized `docker-compose.yml` stack (real Postgres, prod-built Angular bundle served via nginx). Run the same suite against it via `E2E_BASE_URL`.

Both are required, not either/or: local dev catches regressions fast, but only the containerized run exercises real Postgres and the actual prod-built frontend bundle — the source of bugs local dev structurally cannot see (see the Docker/Postgres gotcha below). As more dependencies pick up a live-vs-test split (mail, auth providers, etc.), this is also where that class of gap gets caught going forward.

See [E2E Tests (Playwright)](#e2e-tests-playwright) below for exact commands for both targets.

Fix any failures before moving to step 3.

### Step 3 — Documentation checklist

Before staging, explicitly ask:

1. **README.md** — does this change anything in the features table or architecture notes? If yes, update it in the same commit.
2. **docs/decisions/** — does this introduce or close a meaningful architectural or policy decision? If yes, add a new numbered ADR and update `docs/decisions/README.md`. ADRs are never edited after acceptance — superseded decisions get a new ADR.
3. **Bruno collection** — does this add or change any API endpoints? If yes, create or update the corresponding `.bru` request files in `bruno/` in the same commit.

A commit with no doc or test updates is fine, but the check must be deliberate, not skipped.

## Key Technical Gotchas

### String enums (critical)
The API uses `JsonStringEnumConverter` globally — all enums serialize as strings (`"Active"`, `"Male"`, `"Single"`), **not** integers. Angular `mat-select` option values must use string literals to match:
```html
<!-- CORRECT -->
<mat-option value="Active">Active</mat-option>

<!-- WRONG — will never match API response -->
<mat-option [value]="0">Active</mat-option>
```
`patchValue` from API responses sets string values; integer option values won't match → selects appear blank → data appears not to persist.

### Angular signals (critical)
All state that drives the template **must** be a `signal()`. Plain class properties are invisible to the scheduler. Use `computed()` for derived state. No `BehaviorSubject`, no manual `markForCheck()`.

### JWT role claims
`[Authorize(Roles = "SystemAdmin")]` checks `ClaimTypes.Role` — not the custom `system_admin` claim. Both must be added to the JWT in `GenerateJwtTokenAsync`. `isSystemAdmin` is also included in the auth response payload so the frontend can gate UI without parsing the token.

### Admin-only operations
The following member operations require `[Authorize(Roles = "SystemAdmin")]` at the API level and are hidden behind an `isAdmin()` computed signal in the UI:
- Link user, Unlink user, Send invite (`/members/{id}/link-user`, `/members/{id}/invite`)
- Move organization (`/members/{id}/organization`)
- Status field in member edit form

### Postgres DateTime Kind (critical for seed/dev data)
Postgres rejects writing a `DateTime` with `Kind=Unspecified` into a `timestamp with time zone` column — SQLite (local dev's default) doesn't enforce this at all, so a bad value only surfaces when running against real Postgres. Any hardcoded `DateTime` in seed/dev data (`DatabaseSeeder`, `DevDataSeeder`, `DevSeedData`) must go through `DateTime.SpecifyKind(value, DateTimeKind.Utc)` before being assigned to an entity. This exact bug crashed the API on startup once already — see the Docker/Postgres smoke test below, which is what actually catches this class of bug.

## E2E Tests (Playwright)

### Local dev target

Run E2E tests from `web/five-talents-web/`:

```bash
# The full stack must be running before executing E2E tests.
# In separate terminals:
dotnet run --project src/FiveTalents.Api      # API on http://localhost:5290
cd web/five-talents-web && npm start          # Angular on http://localhost:4200

# Then run E2E tests (from web/five-talents-web/):
npm run e2e

# Headed mode for debugging:
npx playwright test --headed

# Single spec file:
npx playwright test e2e/auth.spec.ts
```

> **Ubuntu 26.04:** Playwright's bundled Chromium is not supported on this OS. The config auto-detects `/usr/bin/chromium-browser`. Install it with `sudo apt install chromium-browser` if missing. On CI (Ubuntu 22.04/24.04) Playwright installs its own binary — no extra config needed.

E2E reports land in `playwright-report/` (gitignored). Test artifacts go to `test-results/` (gitignored). Tests use `reuseExistingServer: true` — if the stack is already running, it will be reused rather than restarted. Each test creates data with a unique timestamp and deletes it on completion.

### Docker/Postgres target

Required before a PR whenever the API or web app changed — see [Before Every Commit](#before-every-commit). Runs the same suite against the containerized `docker-compose.yml` stack instead: real Postgres (not SQLite) and the prod-built Angular bundle served through nginx's `/api/` proxy (not the dev server). This is the only place Postgres-specific behavior and container-packaging bugs get exercised at all — nothing in the automated test suite (`dotnet test`) touches Postgres, and native local dev never runs the prod build.

```bash
# Bring up the full containerized stack (Postgres, Mailpit, api, web).
# --build picks up any local code changes.
docker compose up --build -d

# Confirm all four containers are healthy/up and the api didn't crash-loop.
docker compose ps
docker compose logs api --tail 50

# Run E2E against the container stack instead of native dotnet run/npm start.
# Host/port depend on your Docker context (`docker context ls`) — use localhost
# for a local Docker install, or your remote host's address for a remote context.
cd web/five-talents-web
E2E_BASE_URL=http://localhost:4200 npm run e2e

# Tear down — the stack holds no data worth keeping between runs.
docker compose down -v
```

`E2E_BASE_URL` auto-forces Playwright to a single worker (see `playwright.config.ts`) — concurrent spec files add enough real network latency against a containerized target to produce flaky UI timing failures that never reproduce against localhost.

If the change touches EF migrations or seed data (`DatabaseSeeder`/`DevDataSeeder`), tear down with `docker compose down -v` *before* the `--build` above too, so migrations apply to a genuinely empty database — an already-migrated container can hide a migration that only fails from a clean state.

## Code Coverage

Run coverage locally with the XPlat collector and generate an HTML report:

```bash
# Collect coverage (outputs XML under coverage/raw/)
dotnet test --settings coverlet.runsettings --results-directory coverage/raw

# Generate HTML report (requires dotnet tool restore first)
dotnet tool restore
dotnet reportgenerator -reports:"coverage/raw/**/*.xml" -targetdir:"coverage/html" -reporttypes:Html

# Open the report
xdg-open coverage/html/index.html   # Linux
open coverage/html/index.html       # macOS
```

The `coverage/` directory is gitignored. The `reportgenerator` tool is declared in `.config/dotnet-tools.json`; run `dotnet tool restore` once after cloning. Threshold enforcement (**70% line**) is enforced in CI by parsing the output XML. PostgreSQL migration files are excluded from coverage via `coverlet.runsettings` (auto-generated EF scaffolding). Current coverage is ~86% with exclusions applied.

## Architecture Decision Records

ADRs live in `docs/decisions/`. See `docs/decisions/README.md` for the index. Current range: 0001–0016.
