# ADR-0014: Organization-Scoped Configuration and the Org-First Feature Model

**Status:** Accepted
**Date:** 2026-05-25

## Context

As FiveTalents adds feature modules (giving, events, attendance, groups, volunteers,
communication), each module needs configurable behaviour that varies between
organizations — currency, fiscal year, locale/address format, feature toggles,
integration credentials, and so on.

Two placement options were considered:

1. **System-level `appsettings.json`** — simple but forces all orgs to share the same
   defaults; individual org customisation requires a code change and redeployment.
2. **`OrganizationSettings` entity** — each org carries its own config in the database
   alongside its data, allowing self-service customisation with no deployment.

The existing `Organization` domain entity already models a parent/child hierarchy
(`ParentOrganizationId`, `ChildOrganizations`, `Level`). A corresponding decision is
needed about how parent orgs interact with child org data.

## Decision

**Organization is the primary tenant unit for all features in FiveTalents.**

- All configurable behaviour (locale, currency, feature flags, integration credentials)
  belongs on `OrganizationSettings` — never in `appsettings.json`.
- `appsettings.json` is reserved for infrastructure concerns only: connection strings,
  JWT signing keys, SMTP host, and external service endpoints.
- When a feature module needs a per-org default (e.g. `DefaultCountry` for address
  labels, fiscal year start for giving reports), the field is added to
  `OrganizationSettings` and read via the org context at runtime.
- Parent organizations have read-oriented visibility into child organization data.
  The exact claim and query rules are deferred to the RBAC design (see
  [ADR-0007](0007-jwt-claims-strategy.md) and issue #8), but the data model already
  supports the hierarchy.

## Consequences

- **Feature implementation pattern:** Every new feature that requires per-org
  configuration adds its field(s) to `OrganizationSettings`. Angular services that need
  org config read from the settings endpoint — they never call `IConfiguration`.

- **Issue #44 (international address config):** `DefaultCountry` (ISO 3166-1 alpha-2
  code, e.g. `"US"`, `"GB"`, `"AU"`) is added to `OrganizationSettings`. The Angular
  address-label resolution service and the phone input's default country both derive from
  this field. This becomes the template for any future locale-driven UI behaviour.

- **Issues #1–6 and #46 (feature modules):** Giving records, events, attendance,
  groups, families, volunteers, and communication logs are all scoped to the organization
  they belong to. Cross-org visibility follows the parent/child hierarchy once the RBAC
  layer (#8) is designed.

- **`appsettings.json` stays thin:** Only infrastructure/secrets live there. This keeps
  the system easy to deploy (single build artifact, org config in DB) and allows orgs to
  customise behaviour without a redeployment.
