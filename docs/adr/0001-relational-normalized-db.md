# ADR-0001 — Relational normalized DB, one SQLite file per make

**Status:** Accepted · **Date:** 2026-10

## Context

The source data is a sprawling scan-tool dump: per-make blobs mixing DTCs, live-data formulas, routines, module maps, licensing beans, and a huge string pool. We need a storage model that is queryable by an engine, reviewable in a PR, shippable per-make, and honest about partial knowledge.

Options considered:
1. **Ship the raw JSON** and parse at runtime. Simple, but no integrity, no query, no provenance, and the engine re-does ETL every launch.
2. **One monolithic relational DB** for all makes.
3. **One normalized SQLite per make** (chosen).
4. **A document store** (NoSQL). Flexible but weak on the relational integrity (engine/alias/module FKs) this domain is full of.

## Decision

A **normalized relational schema in SQLite, one file per make** (`diag_<MAKE>.sqlite`). 17 tables + a resolver view ([02](../02-data-model.md)). SQLite because it's a single file, zero-config, embedded in every target platform, trivially hashable, and attachable read-only.

One file *per make* because packs must ship and update independently ([06](../06-oem-pack-model.md)).

## Consequences

**Positive**
- Independent pack shipping/versioning; a bad pack can't poison others.
- Real FK integrity for the engine/alias/module/routine relationships.
- Per-pack integrity hashing is stable (a monolith's hash churns on every edit).
- Users install only the makes they need; offline browse works for free.
- Reviewable: a PR's DB change is a bounded, explainable hash diff.

**Negative / costs**
- Cross-make queries require attaching multiple DBs (acceptable — rare, and the engine handles it).
- Schema migrations must be coordinated across packs via the `schema_version` gate.
- Some duplication of reference tables (`coverage_function`, `provenance_status`) across files — cheap, and keeps packs self-contained.

## Notes

The schema deliberately includes audit (`etl_run`, `content_hash`) and trust (`provenance`, `frame_source`, `verified`) columns from day one — see [ADR-0002](0002-provenance-first.md).
