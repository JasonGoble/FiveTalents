# ADR-0015: Database Provider Strategy — SQLite for Dev, PostgreSQL for Production

**Status:** Accepted
**Date:** 2026-05-27

## Context

Local development previously required a running PostgreSQL instance (either as a Windows service or via Docker). This created friction: contributors needed to install and configure PostgreSQL before they could run the API at all, and the VS Code `Full Stack` launch config had an implicit dependency on an external service that wasn't explicitly documented.

The project also anticipated supporting multiple database providers in the future — the existing infrastructure already used EF Core abstractions, so the underlying ORM was not the constraint.

SQLite is a zero-dependency, file-based database that EF Core supports natively. Using it for local development eliminates the external service requirement without changing the production configuration.

## Decision

Local development uses **SQLite** by default. Production deployments use **PostgreSQL**.

The active provider is determined by the `DatabaseProvider` configuration key:
- `"Sqlite"` (default when key is absent) → SQLite
- `"Postgres"` → PostgreSQL (Npgsql)

`appsettings.json` sets `"DatabaseProvider": "Postgres"` so that production Render deployments use PostgreSQL without any environment-variable gymnastics. The gitignored `appsettings.Development.json` overrides this to `"Sqlite"` with a local `Data Source=FiveTalents.db` connection string, meaning `dotnet run` works with no external services.

Migrations are managed per-provider via a separate `FiveTalents.Migrations.<Provider>` project:
- `FiveTalents.Migrations.Sqlite` — SQLite-native migrations (`TEXT`, `INTEGER`, `Sqlite:Autoincrement`), with a `SqliteDesignTimeFactory` so `dotnet ef` can target the project directly
- `FiveTalents.Infrastructure` — continues to host PostgreSQL migrations (Npgsql types)

Each provider assembly is added to the solution and referenced by `FiveTalents.Api` so both migration sets are available at runtime and design time.

## Consequences

**Easier:**
- Zero-dependency local dev: clone, restore, `dotnet run` — no PostgreSQL install required
- Adding a new provider (e.g. MySQL, SQL Server) follows a clear pattern: new `FiveTalents.Migrations.<Provider>` project + factory + config value
- Dev and prod schemas stay in sync via their respective migration sets; no shared migration file that must satisfy both dialects

**Harder:**
- Two migration sets must be maintained in parallel. A model change requires running `dotnet ef migrations add` twice — once per provider — and keeping them semantically aligned
- SQLite behavioral differences (no `ALTER COLUMN`, limited constraint support) mean some migrations require table-rebuild workarounds that Postgres handles natively; these cases must be handled in the SQLite migration manually
- Developers must remember which `--project` flag to use when generating migrations (`FiveTalents.Migrations.Sqlite` for SQLite, `FiveTalents.Infrastructure` for Postgres)
