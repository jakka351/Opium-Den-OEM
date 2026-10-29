# ADR-0002 — Provenance-first data model

**Status:** Accepted · **Date:** 2026-10

## Context

The data comes from reverse-engineering a closed tool. Some of it is fully understood (DTC text), some is framed but not decoded (module structure), some needs a vendor interpreter we don't have (blocked calc-scripts), and some is just unconfirmed. A naïve schema stores all of it identically and the engine then presents a guess with the same confidence as a measurement. In a tool that touches brakes and airbags, that's not a bug — it's a hazard and a credibility killer.

## Decision

**Every datum carries its provenance, and trust is a first-class, queryable property** — not a comment, not a convention. Concretely:

- `provenance_status` enum on every content row (`NOW-EXTRACTED`, `NOW-STRUCTURE-ONLY`, `BLOCKED-CALCSCRIPT`, `BLOCKED-RPC`, `GATED-LEGAL/IP`, `UNVERIFIED`).
- Routines additionally carry `frame_source` (origin of the wire template) and `verified` (proven on a bus or not).
- `verified=1` can **only** be set by a human attaching a bus trace — never by ingestion.
- The engine's "can we drive this?" is **computed live** from these fields (the firing rule, [03](../03-provenance-trust-coverage.md)), and the UI badges everything with its trust state.

## Consequences

**Positive**
- The tool never lies about confidence. A guessed scaling renders differently from a measured one.
- Empty/partial coverage becomes *informative* (why is it empty?) instead of a silent zero.
- Contribution has a clear target: move data up the trust ladder, visibly.
- Safety and legal gating ([ADR-0004](0004-safety-gating.md)) hook straight into the same fields.

**Negative / costs**
- More columns, more discipline. Contributors must pick the honest status (CI enforces the invariants — no `verified=1` without a trace, etc.).
- The UI must be designed to *show* trust, not hide it — more work than a plain table.

## Notes

This is the decision that makes the project trustworthy enough to touch a real car. If a future change ever tempts someone to "just fill in the blanks" without provenance, that violates this ADR and should be rejected.
