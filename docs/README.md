# OpenDEN — Engineering & Design Specification

> **Opium-Den-OEM** is the data dump and bootstrapper. **OpenDEN** (working codename — *Open Diagnostic ENgine*) is the open-source diagnostic tool these documents specify: a community-built, cross-platform diagnostic application for Chinese-market vehicles, driven by the normalized diagnostic databases in this repository.

This `docs/` tree is the design and engineering spec for turning the raw data in [`/Data`](../Data) and the transport code in [`/src`](../src) into a working, maintainable, community-developed tool. It is written to be read by contributors, not just the maintainer: every component has a clear boundary, every table has a documented meaning, and every sensitive capability has an explicit governance model.

## Why this exists

Chinese OEMs now ship huge volumes into AU/NZ, SEA, MEA, LATAM and Europe, and independent workshops have almost no affordable diagnostic coverage for them. The expensive aftermarket tablets (Autel, Launch, XTOOL, Topdon) *do* cover them — and the diagnostic knowledge inside those tools (DTC catalogues, live-data definitions, service routines, module maps) is largely just **structured data**. This project extracts that structure, normalizes it into an open schema, and specifies a clean, legal, community-maintainable engine to drive it.

The dataset already in this repo is substantial:

| Metric | Count |
|---|---|
| OEM make databases | 15 |
| OEM / badge entries | 36 |
| Vehicle rows | 36 |
| ECU module rows | 470 |
| Decoded DTCs (fault codes) | **177,279** |
| Live-data parameters (datastreams) | **128,306** |
| Service routines | 646 |
| Function classes | 79 |

## Read in this order

| # | Document | What it covers |
|---|---|---|
| 00 | [Overview & Scope](00-overview.md) | Vision, scope, non-goals, principles, the legal/ethical stance |
| 01 | [System Architecture](01-architecture.md) | Layered architecture, components, data flow, tech stack |
| 02 | [Data Model](02-data-model.md) | The normalized diagnostic schema, table-by-table, with the ERD |
| 03 | [Provenance, Trust & Coverage](03-provenance-trust-coverage.md) | How "known vs. blocked vs. unverified" is tracked and surfaced |
| 04 | [Transport & Hardware Abstraction](04-transport-hal.md) | J2534, SocketCAN, ELM327, DoIP; the ISO-TP / session layers |
| 05 | [Protocol & Service Layer](05-protocol-services.md) | UDS/KWP service model, routines, the datastream formula engine |
| 06 | [OEM Pack Model](06-oem-pack-model.md) | Engines, aliases, packs, and how the community ships coverage |
| 07 | [Safety, Security & Legal](07-safety-security-legal.md) | SecurityAccess governance, the restraints guard, gated functions, clean-room policy |
| 08 | [Ingestion Pipeline (ETL)](08-ingestion-pipeline.md) | Raw dump → normalized DB, hashing, reproducibility |
| 09 | [Contributing](09-contributing.md) | Repo layout, data-pack contribution flow, verification tiers, review bar |
| 10 | [Roadmap](10-roadmap.md) | Phased delivery from read-only MVP to verified bidirectional routines |
| — | [Glossary](GLOSSARY.md) | Domain terms and acronyms |
| — | [Architecture Decision Records](adr/README.md) | The "why" behind the big choices |

## Status

This is a **specification and bootstrap**, not a finished application. The data layer is real and populated. The transport layer (`/src/PassThru`) is real C#. The engine, ETL, and app described here are the build target for the community. See the [Roadmap](10-roadmap.md) for what "done" means at each phase.

## License & governance note

Diagnostic *facts* (a DTC number means X; a PID scales like Y) are not the sensitive part. Cryptographic seed/key material, vendor binaries, and anything that enables theft, fraud, or unsafe airbag work **are**, and this spec deliberately designs the engine so those are gated, not shipped. See [07 — Safety, Security & Legal](07-safety-security-legal.md) before contributing anything in those areas.
