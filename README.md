# FiveTalents

> *Faithful stewardship for growing churches.*

Inspired by the Parable of the Talents (Matthew 25:14–30), FiveTalents is a free, open-source church management platform built for small-to-mid-sized churches — Anglican, Presbyterian, Lutheran, Methodist, and Reformed traditions especially. It gives any congregation modern tools to shepherd their people, coordinate ministries, and steward resources faithfully, without the complexity or cost of enterprise ChMS software.

> **A note on how this was built:** This project is an experiment in *vibe coding* — the practice of building software through a collaborative, conversational workflow with an AI pair programmer (Claude). Every feature, migration, and architectural decision in this codebase was shaped through that dialogue. The goal is not just a working product, but a demonstration that thoughtful AI-assisted development can produce clean, maintainable, production-quality code.

---

A full-stack church management system built with .NET 10 and Angular 21.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| API | .NET 10, ASP.NET Core, MediatR, FluentValidation |
| ORM | Entity Framework Core 10, SQLite (dev) / PostgreSQL (prod) |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Frontend | Angular 21, Angular Material |
| Architecture | Clean Architecture, CQRS |

## Project Structure

```
FiveTalents/
├── src/
│   ├── FiveTalents.Domain/          # Entities, enums, domain logic
│   ├── FiveTalents.Application/     # CQRS commands/queries, DTOs, interfaces
│   ├── FiveTalents.Infrastructure/  # EF Core, Identity, external services
│   └── FiveTalents.Api/             # Controllers, middleware, startup
└── web/
    └── five-talents-web/            # Angular 21 SPA
```

## Features

| Module | Status |
|--------|--------|
| Organizations | ✅ Hierarchy tree with settings |
| Members | ✅ Profiles, multiple addresses/emails/phones (typed + primary flag), privacy controls, user account linking & invites, admin-only status & org management (move between orgs) |
| Families | ✅ Family grouping with customizable roles (adult/child flag), membership management from both family and member views |
| Groups & Ministries | ✅ Types, leaders, schedules, capacity |
| User Accounts | ✅ JWT auth, end-to-end invite & account setup flow, My Profile page |
| Attendance | 🔲 Planned |
| Giving | 🔲 Planned |
| Events | 🔲 Planned |
| Sermons | 🔲 Planned |
| Volunteers | 🔲 Planned |
| Communication | 🔲 Planned |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+ / npm 11+](https://nodejs.org)
- [Angular CLI](https://angular.io/cli) — `npm install -g @angular/cli`
- [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) — `dotnet tool install --global dotnet-ef`

Track A (SQLite) needs no additional services. Track B (Docker/Postgres) requires [Docker Desktop](https://docs.docker.com/get-docker/).

## Getting Started

The default setup (Track A) uses SQLite with no external dependencies. Track B shows Docker for self-hosting or local PostgreSQL. Both share the same API and frontend steps that follow.

---

### Track A — SQLite (default, zero dependencies)

The default local dev setup uses **SQLite** — no external database needed. The gitignored `appsettings.Development.json` is pre-configured with:

```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=FiveTalents.db"
  }
}
```

The database file is created automatically by EF Core on first run. No external services are required — emails are written as `.eml` files to `logs/emails/` instead of being sent over SMTP.

**To use PostgreSQL locally instead**, set `"DatabaseProvider": "Postgres"` and supply a Postgres connection string in `appsettings.Development.json`, then apply the Postgres migrations (see step 1 below).

---

### Track B — Docker (Windows 11, macOS, Linux)

Requires [Docker Desktop](https://docs.docker.com/get-docker/) with Compose. Start only the backing services — the API and Angular app still run natively.

```bash
docker compose up db mailpit -d --wait
```

This starts PostgreSQL on port **5433** and Mailpit on port **1025** (UI at `http://localhost:8025`).

> **Self-hosting / full-stack Docker:** `docker-compose.yml` also defines `api` and `web` services if you want to run the entire stack in containers (e.g. on your own server). Run `docker compose up` to start all four services; the API will be on port 5000 and the frontend on port 4200. This is separate from the Render deployment, which uses `render.yaml` with individual Dockerfiles.

---

### 1. Apply migrations

**SQLite (default):**

```bash
dotnet-ef database update --project src/FiveTalents.Migrations.Sqlite --startup-project src/FiveTalents.Api
```

**PostgreSQL:**

```bash
dotnet-ef database update --project src/FiveTalents.Infrastructure --startup-project src/FiveTalents.Api
```

### 2. JWT Secret

Replace the placeholder secret in `src/FiveTalents.Api/appsettings.json` with a strong random key (≥ 32 characters):

```json
"JwtSettings": {
  "Secret": "your-strong-random-secret-here"
}
```

### 3. Start the API

```bash
dotnet run --project src/FiveTalents.Api
# Listening on http://localhost:5290
# OpenAPI docs: http://localhost:5290/openapi/v1.json
```

### 4. Start the Frontend

```bash
cd web/five-talents-web
npm install
npm start
# App running at http://localhost:4200
```

### VS Code (recommended)

Press **F5** (or select **Full Stack** from the Run & Debug panel). VS Code will build the API and launch both servers with Chrome and .NET debuggers attached. Angular is stopped automatically when the debug session ends.

- **SQLite (Track A default):** No external services needed — just press F5.
- **PostgreSQL locally:** Start Postgres first (service or `docker compose up db -d`), then press F5.

For production, set real SMTP credentials in `appsettings.json`:

```json
"SmtpSettings": {
  "Host": "smtp.yourprovider.com",
  "Port": 587,
  "FromAddress": "noreply@yourdomain.com",
  "FromName": "FiveTalents",
  "Username": "your-smtp-username",
  "Password": "your-smtp-password"
}
```

## Default Seed Data

On first run the database is seeded with:

- A root organization (*My Church*)
- An admin user — `admin@FiveTalents.local` / `Admin1234!`
- Seven default group types (Small Group, Ministry Team, Bible Study, Prayer Group, Youth, Children, Leadership Team)

## Troubleshooting

### Direct URL navigation returns 404

FiveTalents is a Single Page Application. The browser only ever loads `index.html` once; after that Angular's router handles all navigation client-side. If you type a URL directly into the address bar (e.g. `/members/42`) the web server receives a request for a path that doesn't exist as a file, so it returns 404.

The fix is a catch-all rewrite rule that sends every unmatched path to `index.html`:

| Hosting | What to do |
|---------|------------|
| **Render static** | `routes` rewrite in `render.yaml` — already configured |
| **IIS** | `web.config` with URL Rewrite module — included in every build output via `public/web.config` |
| **Nginx** | Add `try_files $uri $uri/ /index.html;` inside the `location /` block |
| **Apache** | Add `FallbackResource /index.html` to `.htaccess` |
| **Local dev** | Angular's dev server handles this automatically — no action needed |

## Architecture Notes

- **Clean Architecture** — dependency flow is strictly Domain → Application → Infrastructure/API.
- **CQRS via MediatR** — every read is a `Query`, every write is a `Command`; no business logic in controllers.
- **Soft deletes** — `Member`, `Group`, `GroupType`, `Family`, `Sermon`, and `ChurchEvent` use an `IsDeleted` flag with a global EF query filter.
- **Auditing** — all entities inherit from `AuditableEntity` which stamps `CreatedAt` / `UpdatedAt` and scopes records to an `OrganizationId`.
- **Database providers** — SQLite in local dev (zero dependencies); PostgreSQL in production. The active provider is set via `DatabaseProvider` in configuration. Migrations live in separate assemblies: `FiveTalents.Migrations.Sqlite` and `FiveTalents.Infrastructure` (Postgres). See [ADR-0015](docs/decisions/0015-database-provider-strategy.md).
- **String enums** — the API serializes all enums as strings (`JsonStringEnumConverter`).
- **Angular signals** — all component state uses `signal()` / `computed()`; no `BehaviorSubject` or manual change detection.
- **Role-gated admin actions** — Destructive member-account operations (unlink) are restricted to the `SystemAdmin` role at the API level (`[Authorize(Roles = "SystemAdmin")]`). The `isSystemAdmin` flag returned by auth endpoints lets the frontend hide admin controls for regular users.

## Contributing

1. Create a branch: `feature/<issue#>-<slug>` or `fix/<issue#>-<slug>`
2. Commit your changes (include `Closes #N` to auto-close the issue on merge)
3. Open a pull request against `main`
