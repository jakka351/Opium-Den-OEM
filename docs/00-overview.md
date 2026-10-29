# 00 — Overview & Scope

## 1. Vision

A free, open-source, cross-platform diagnostic application that gives independent workshops and enthusiasts honest coverage of Chinese-market vehicles — read fault codes, watch live data, run the safe service routines (oil reset, EPB, throttle relearn, bleed procedures), and identify modules — using an open, inspectable, community-maintained knowledge base instead of a locked aftermarket tablet.

The guiding idea from the repo README stands: *it's not that hard to build tooling for these vehicles.* The hard part was never the protocol — it's **CAN and UDS like everything else**. The hard part is the knowledge base: tens of thousands of DTC definitions, PID scalings, module maps and routine sequences per make. This project opens that knowledge base and specifies the engine around it.

## 2. What it is

- A **diagnostic engine** that speaks UDS (ISO 14229) and legacy KWP2000 over CAN (ISO 15765-2 / ISO-TP), with a path to DoIP (ISO 13400) for newer EV/zonal architectures.
- A **normalized, open diagnostic database** (SQLite, one file per make) describing modules, DTCs, live-data parameters, and service routines — with full provenance tracking.
- A **hardware abstraction layer** that runs over cheap and professional interfaces alike: SAE J2534 PassThru devices, SocketCAN adapters, and ELM327-class dongles.
- A **pack model** so the community ships and updates per-OEM coverage independently of the engine.
- A **reference application** (GUI + CLI) demonstrating the engine.

## 3. What it is NOT (non-goals)

These are explicit. They keep the project legal, safe, and focused.

| Non-goal | Why |
|---|---|
| A key-programming / immobiliser-defeat tool | Theft-enabling. IMMO/key functions are modelled as **metadata only** and gated off by default. See [07](07-safety-security-legal.md). |
| An odometer / mileage-correction tool | Fraud, illegal in most jurisdictions (incl. AU). ODOMETER/EEPROM write routines are catalogued for identification but **not shipped as executable sequences**. |
| A redistribution point for OEM or aftermarket binaries | `.so`/`.p12`/calibration blobs stay out. The DB stores *facts and structure*, not vendor code. |
| A seed/key algorithm library | Clean-room structural slots only; no lifted crypto. See [ADR-0004](adr/0004-safety-gating.md). |
| An ECU flashing / tuning suite | Out of scope for v1. Flash/programming is high-risk and OEM-specific; may be revisited post-1.0 under strict gating. |
| A cloud service / telemetry harvester | The tool is local-first. No phone-home. |

The tool is a **right-to-repair diagnostic**, not a bypass kit. That distinction is designed into the schema and the engine, not just stated here.

## 4. Design principles

1. **Protocol honesty.** Chinese vehicles are not exotic. They run the same ISO stack as everyone else. The engine targets the standards first and treats OEM quirks as data, not special-cased code.
2. **Provenance over confidence.** Every datum carries where it came from and how much we trust it. The UI never shows a guessed value as if it were measured. See [03](03-provenance-trust-coverage.md).
3. **Data/engine separation.** Coverage ships as data packs. Adding a make must never require recompiling the engine. See [06](06-oem-pack-model.md).
4. **Safe by default.** Restraints (SRS/airbag) modules and theft/fraud-adjacent functions are gated behind explicit, logged, opt-in confirmation — or omitted entirely. See [07](07-safety-security-legal.md).
5. **Reproducible ingestion.** The path from raw dump to shipped DB is scripted, hashed, and re-runnable by anyone. No magic maintainer box. See [08](08-ingestion-pipeline.md).
6. **Cheap hardware works.** A $30 ELM327 should do read-only diagnostics. Routines and multi-frame work want a real J2534 or SocketCAN device, and the engine degrades gracefully. See [04](04-transport-hal.md).
7. **Community-legible.** A new contributor can read these docs and the schema and understand the whole system in an afternoon.

## 5. Target users

- **Independent workshops** servicing BYD, Chery, GWM, Geely, MG/SAIC, Changan, LDV/Maxus etc. without paying per-make tablet subscriptions.
- **Enthusiasts & owners** wanting to read their own faults and live data.
- **Researchers & educators** who want an open, inspectable diagnostic stack.
- **Contributors** who can add a make by producing a data pack, without touching engine code.

## 6. Coverage today

15 make databases are populated (see [02 — Data Model](02-data-model.md) for the full breakdown). Highlights: Chery (+EXEED/JETOUR/OMODA family), BYD, Changan, Geely, Great Wall (GWM/Haval/Tank), SAIC (MG/Maxus), SGMW (Wuling/Baojun), Dongfeng (DFFS/DFFX/DFXK), BAIC, Lynk&Co, plus Mahindra and SsangYong carried along from the same source family.

DTC and live-data coverage is deep and ready to use. Routines, module CAN IDs, and anything requiring the vendor's blocked script interpreter are **structure-only** until decoded by the community — the schema tracks exactly which is which.

## 7. Legal & ethical stance (read this)

This project exists to support **repair, maintenance, and diagnosis** — activity protected under right-to-repair norms and, in Australia, the *Motor Vehicle Information Scheme* (MVIS). It is **not** a vehicle-theft, odometer-fraud, or anti-theft-defeat project, and the architecture is built to keep it that way:

- No lifted cryptographic key algorithms. Structural slots and clean-room references only.
- No executable immobiliser/key-learn or odometer-write sequences shipped.
- SRS/restraints work gated behind explicit safety confirmation.
- No vendor binaries redistributed.

Contributors who want to push in those directions should read [07](07-safety-security-legal.md) first; PRs that cross these lines will be declined regardless of technical merit. This is what keeps the project defensible and useful to the people it's actually for.

---

Next: [01 — System Architecture](01-architecture.md)
