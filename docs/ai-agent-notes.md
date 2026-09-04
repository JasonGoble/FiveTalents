# AI Agent Notes — FiveTalents

Durable context for AI coding assistants working in this repo, beyond what's in `CLAUDE.md`.
Migrated from Claude Code memory during the move to GitHub Copilot, 2026-09-03 — treat any
PR/issue numbers below as historical, not necessarily current; verify against `gh` milestones/issues.

## Brand (docs/brand/media-kit.md, ADR-0011 rename decision)
- Renamed from ChurchManager → FiveTalents, May 2026 (Parable of the Talents, Matt 25:14-30).
- Tagline: "Faithful stewardship for growing churches."
- Audience: small-to-mid liturgical/historically-rooted churches (75-800 members) — Anglican,
  Presbyterian, Lutheran, Methodist, Reformed. Underserved by existing ChMS products.
- Tone: calm, trustworthy, pastoral, understated. Avoid startup jargon, megachurch aesthetics,
  "AI-first" framing. Prefer stewardship/ministry language in copy/issues/feature names
  ("shepherd people" not "manage contacts").

## Angular responsive patterns (established in issues #33, #35; apply to all new components)
- Breakpoint: mobile `< 768px`; dialog forms `< 600px`.
- Use an `isMobile` signal via `BreakpointObserver` + `toSignal`:
  ```ts
  private bp = inject(BreakpointObserver);
  isMobile = toSignal(this.bp.observe('(max-width: 767px)').pipe(map(r => r.matches)), { initialValue: false });
  ```
- Never render a multi-column `mat-table` on mobile — switch to a card list with
  `@if (isMobile()) { ...cards... } @else { ...mat-table... }`.

## Date inputs
- Always use the custom `DateInputDirective` (`src/app/shared/directives/date-input.directive.ts`),
  selector `input[matDatepicker]` (auto-applies, no attribute needed). Zero-dependency by design —
  do not add `ngx-mask` or similar libraries for date masking.

## Bruno API tests
- Every new/changed endpoint needs `.bru` files in `bruno/` in the same commit — beyond happy path:
  missing required fields, invalid IDs, unauthorized access, duplicate/conflict cases, query-param
  edge cases. One `.bru` file per logical scenario, not cramped multi-case files.
- `bru.setEnvVar` in pre-request scripts does NOT affect the current request's body — use
  `{{$timestamp}}` / `{{$guid}}` built-ins for unique values in request bodies instead.
