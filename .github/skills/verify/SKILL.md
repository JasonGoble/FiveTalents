---
name: verify
description: Build/launch/drive recipe for verifying FiveTalents changes end-to-end — API (.NET) and frontend (Angular) surfaces.
---

# Verifying FiveTalents changes

Full stack needs no external services in dev — SQLite is the default database, dev emails are
written as `.eml` files to `logs/emails/`. Press F5 in VS Code ("Full Stack") to launch both with
debuggers attached, or start manually:

```bash
dotnet run --project src/FiveTalents.Api      # http://localhost:5290
cd web/five-talents-web && npm start          # http://localhost:4200
```

Default admin login: `admin@FiveTalents.local` / `Admin1234!`.

## Backend surface (API)

Drive it with `curl`, not by importing handlers directly. Auth first, then use the token:

```bash
curl -s -X POST http://localhost:5290/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@FiveTalents.local","password":"Admin1234!"}' | python3 -m json.tool

curl -s http://localhost:5290/api/members -H "Authorization: Bearer <token>" | python3 -m json.tool
```

Prefer the Bruno collection (`bruno/`) over ad hoc `curl` when checking an existing endpoint —
it already has request/scenario coverage (happy path, missing fields, unauthorized, conflicts):

```bash
npx @usebruno/cli run . --env local -r
```

## Frontend surface (Angular)

Playwright **is** a project dependency here (`@playwright/test`) — unlike FiveTalents.Calendar,
no scratchpad install needed. Full stack must already be running (see above); e2e reuses an
existing server (`reuseExistingServer: true` in `playwright.config.ts`) rather than restarting it.

```bash
cd web/five-talents-web
npm run e2e                       # full suite
npx playwright test --headed      # headed, for visual debugging
npx playwright test e2e/auth.spec.ts   # single spec
```

> **Ubuntu 26.04:** Playwright's bundled Chromium isn't supported. Config auto-detects
> `/usr/bin/chromium-browser`; install with `sudo apt install chromium-browser` if missing.

Each e2e test creates data with a unique timestamp and deletes it on completion — don't assume a
clean DB between runs, and don't hand-craft cleanup for test data on failure without checking
whether the test's own teardown already handles it.

## Cleanup

Confirm ports are actually dead after killing background dev servers — don't assume `pkill`/`lsof`
succeeded silently. Same caveat as any `dotnet run`-based service: it forks the compiled binary as
a child process, so killing the `dotnet run` wrapper PID can leave a stale server still listening
and serving old output. Verify with `ss -tlnp | grep <port>`, kill the real PID if still alive.

## Gotchas

- All enums serialize as PascalCase strings (`JsonStringEnumConverter`) — match e.g. `"Active"`,
  not `"active"` or `0`, when asserting on raw API JSON or setting `mat-select` values.
- `ContactTypes` are seeded with fixed IDs: 1–4 (Address), 5–7 (Email), 8–11 (Phone) — use the
  correct category ID when creating/verifying member contacts via the API directly.
