# ADR-0018: Render Deployment Health Check Before Push

**Status:** Accepted
**Date:** 2026-09-07

## Context

[ADR-0015](0015-database-provider-strategy.md) established PostgreSQL as the production database, provisioned on Render. [ADR-0017](0017-docker-postgres-e2e-parity-gate.md) added local pre-merge coverage for Postgres-specific behavior — but neither addresses the production database's *availability*, which turned out to be its own failure mode.

Render's free-tier Postgres plan auto-deletes the database roughly 30 days after creation (`expiresAt` in Render's own API, "applies to free tier databases only"). `render.yaml` auto-deploys both `fivetalents-api` and `fivetalents-web` on every push to `main` (`autoDeploy: yes`, `autoDeployTrigger: commit`) regardless of what changed. On 2026-09-07, `fivetalents-db` had quietly expired; the next routine push redeployed `fivetalents-api`, which then crash-looped trying to reach a database that no longer existed — a production outage with no code change to blame and nothing in the repo or test suite that could have caught it.

The fix itself is a single click: Render Dashboard → Blueprints → the FiveTalents blueprint → **Manual Sync**. This recreates the missing `fivetalents-db` resource and automatically rewires `fivetalents-api`'s `fromDatabase` connection string, then redeploys. The gap was detection, not remediation — nothing prompted a check until the API was already down.

An attempt was made to automate detection via Render's own REST API (which exposes `expiresAt` directly). A freshly generated API key authenticated successfully exactly once, then every subsequent request — including a byte-for-byte retry of the same successful call, across multiple endpoints — returned an empty-body `400` from Cloudflare's edge. Render's key still showed active and unrevoked in the dashboard, and Render's own status page reported the API fully operational with no incidents. The cause was never isolated, so the Render API was dropped as the detection mechanism rather than debugged further mid-incident.

GitHub's own Deployments API — already populated by Render's native GitHub integration, and already authenticated via `gh` in every session — turned out to be a reliable substitute:
- `main - fivetalents-db`'s most recent deployment `created_at` timestamp marks when the database was last (re)created (Render only records a deployment for this environment when the resource is actually created or recreated by a sync, not on every commit).
- `main - fivetalents-api`'s most recent deployment status (`success` / `failure`) reflects whether the last real deploy actually came up healthy.

## Decision

Before pushing to `main` / opening a PR, check Render's deployment health via `gh api repos/JasonGoble/FiveTalents/deployments` (documented as `CLAUDE.md`'s new "Before Every Commit" Step 1):

1. **Preventive** — how many days old is `fivetalents-db`'s last (re)creation. Flag when it's approaching Render's ~30-day free-tier expiry.
2. **Reactive** — what state was `fivetalents-api`'s most recent deploy. Flag on `failure` regardless of the age check, as a safety net for causes other than database expiry.

Either signal tripping means: run the Blueprint Manual Sync **before** merging, so a new PR never lands on top of an already-broken production deploy. This is a manual pre-push check, not (yet) a CI gate — consistent with ADR-0017's Docker/Postgres gate, `gh-execute`'s existing "follow the target repo's own pre-commit checklist" step picks it up automatically via `CLAUDE.md`, with no tooling change required.

## Consequences

**Easier:**
- The specific failure mode that caused the 2026-09-07 outage (expired free-tier Postgres, silently redeployed into) is now checked *before* every merge, not discovered live in production
- No Render API key management needed — reuses `gh`'s existing GitHub authentication, sidestepping the unresolved Render API access issue entirely
- The fix path (Blueprint Manual Sync) was already validated live during the incident, so the checklist step points at a known-working remediation, not a guess

**Harder:**
- This is a manual step, not CI-enforced — it depends on whoever pushes actually running it, same limitation ADR-0017 already accepts for its own gate
- The "~25 days" warning threshold in `CLAUDE.md` is a heuristic against Render's stated free-tier window, not a value Render exposes directly through GitHub's API — if Render changes that window, the threshold needs a manual update
- Doesn't address the root cause. Upgrading `fivetalents-db` to a paid Render plan would remove free-tier expiry entirely and make this whole check unnecessary; that tradeoff (small recurring cost vs. an ongoing manual check) was not decided here and remains open
- The unresolved Render API key behavior means a more direct check (`expiresAt` straight from Render) isn't available; if that's ever debugged, it could replace the day-count heuristic with an exact value
