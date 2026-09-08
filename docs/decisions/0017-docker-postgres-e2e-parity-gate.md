# ADR-0017: Docker/Postgres Parity Gate for E2E Tests

**Status:** Accepted
**Date:** 2026-09-07

## Context

[ADR-0015](0015-database-provider-strategy.md) made SQLite the local dev default specifically to remove the external-service dependency from day-to-day work, with PostgreSQL reserved for production. [ADR-0016](0016-testing-strategy.md) built the E2E strategy on top of that: both servers started natively (`dotnet run` + `npm start`), which means every E2E run — local and in CI — has only ever exercised SQLite and the Angular dev server, never PostgreSQL or the prod-built frontend bundle.

That gap turned out to be a real source of bugs, not a theoretical one. While smoke-testing the existing `docker-compose.yml` (previously scoped to Render deployment only, per the note in `CLAUDE.md`) against a personal remote Docker host, dev-data seeding crashed the API outright: seeded `DateTime` values with `Kind=Unspecified` are silently accepted by SQLite but rejected outright by Postgres for `timestamp with time zone` columns. Nothing in the SQLite-only automated suite (unit, integration, or E2E) could have caught this — it is invisible by construction to every test that only ever talks to SQLite.

The same reasoning extends past dates: the native local/E2E stack also never exercises the prod-built Angular bundle (nginx-served, optimized, relative `/api` base URL) or the container networking between `web` and `api`. As the project takes on more dependencies with a real live-vs-test split (mail providers, auth providers, etc.), this is the structural gap where that whole class of bug will keep hiding.

## Decision

E2E tests now run against **two** targets whenever a change touches the API or the Angular app, both required, not either/or:

1. **Local dev target** (unchanged from ADR-0016) — native `dotnet run` + `npm start`, SQLite. Fast, no build step, the default `npm run e2e`.
2. **Docker/Postgres target** (new) — the existing `docker-compose.yml` stack (Postgres, Mailpit, `api`, `web` behind nginx), run via `docker compose up --build -d`. The same Playwright suite runs against it by setting `E2E_BASE_URL` to the stack's `web` URL instead of letting the config spawn native servers.

`playwright.config.ts` was extended (not replaced) to support this: `E2E_BASE_URL` overrides `baseURL` and skips the `webServer` auto-start blocks entirely, so the local-dev path is unaffected when the variable is unset. It also forces `workers: 1` when set — real network latency against a containerized target exposed a Material dropdown timing failure under concurrent spec execution that never reproduces against localhost or in isolation; serializing keeps the remote run deterministic without weakening the local-dev run's parallelism.

`docker-compose.yml`'s role changes accordingly: it is no longer solely deployment tooling for Render/self-hosting, it is also the only place Postgres-specific behavior and containerized packaging get exercised pre-merge. When a change touches EF migrations or seed data, the stack should additionally be brought up from a clean volume (`docker compose down -v` before `up --build`), since an already-migrated container can hide a migration that only fails from an empty database — which is exactly how the triggering bug was found.

This decision only extends ADR-0016's E2E section; the unit/integration testing strategy, coverage tooling, and CI job structure it documents are unaffected and remain accurate.

## Consequences

**Easier:**
- Postgres-only defects (strict `DateTimeKind` handling, dialect differences, connection/pooling behavior) are caught before merge instead of surfacing only in a real deploy
- The prod-built Angular bundle and nginx's `/api/` proxy get exercised by the same E2E suite, rather than only ever seeing the dev server
- Future live-vs-test dependency splits (mail, auth, etc.) inherit this same containerized-parity check rather than needing a new mechanism each time
- `gh-execute`'s existing "follow the target repo's own pre-commit checklist" step picks this up automatically via `CLAUDE.md` — no tooling change needed there

**Harder:**
- Every API/web-touching PR now pays for a container image build and a second full E2E run, not just the fast local one — meaningfully slower than the local-only gate ADR-0016 established
- `docker-compose.yml` now has two audiences (Render deployment, and this local parity gate) that must be kept satisfied together — a change convenient for one could regress the other
- This gate is currently manual/local only (part of the pre-commit checklist in `CLAUDE.md`, followed by contributors and `gh-execute`), not yet wired into GitHub Actions CI. CI still only runs the SQLite-backed suites per ADR-0016's job structure; adding a CI-side Postgres/Docker job is a natural follow-up but out of scope here
