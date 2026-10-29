# ADR-0003 — C# engine over a transport HAL; Python ETL

**Status:** Accepted · **Date:** 2026-10

## Context

Two very different jobs: (1) an online **engine** that talks to vehicles in real time across varied hardware, and (2) an offline **ETL** that chews through messy source dumps. They have opposite constraints — the engine wants native driver access, tight types, and determinism; the ETL wants fast iteration on parsing and big-number/formula wrangling.

We also already have a substantial, good-quality **C# J2534 + ELM327 transport** in `/src/PassThru` (`IJ2534Extended` with the full ioctl/periodic/flow-control surface, a device finder, and an ELM327 transport). Throwing that away would be daft.

## Decision

- **Engine in C# / .NET**, building on the existing transport code.
- **All hardware behind one `ITransport` HAL** with a `TransportCaps` capability flag set ([04](../04-transport-hal.md)); backends: J2534, SocketCAN, ELM327, (future) DoIP. The engine binds to the interface, never to a device.
- **ETL in Python**, decoupled by the SQLite file — it writes packs offline; the engine only ever reads them.
- **The SQLite schema + ISO-TP/UDS contract are the real API.** Alternative-language engines (Python/Rust) against the same contract are first-class, not forks.

## Consequences

**Positive**
- Reuses the existing, proven transport layer; native Windows J2534 driver access where the pro hardware lives.
- One place (`ITransport`) to add hardware; capability flags drive graceful degradation (ELM327 = read-only) and the Safety Gate.
- Python keeps ETL iteration fast; the file boundary means ETL churn never destabilizes the engine.
- `RemotePassThru` relay seam (already hinted in the C#) enables networked/bench setups for free.

**Negative / costs**
- Two languages in the repo (C# + Python). Mitigated: they meet only at the SQLite file, with no shared runtime.
- .NET cross-platform story for the GUI needs care (the core + CLI are portable; GUI toolkit choice is deferred).
- Capability-based degradation is more work than assuming one device, but it's the honest model.

## Notes

If the community prefers a different engine language long-term, this ADR explicitly blesses that — provided it honours the schema and the Safety Gate ([ADR-0004](0004-safety-gating.md)). The contract is the data + protocol, not the implementation.
