# 10 — Roadmap

Phased so that each phase ships something useful on its own. The ordering is deliberate: **read-only value first** (works on cheap hardware, zero risk), then verified bidirectional work, then advanced architectures. Nothing risky ships before the safety machinery that governs it.

## Phase 0 — Foundation (data + transport in hand) ✅ mostly done

- [x] Normalized schema defined and populated (15 makes, 177k DTCs, 128k datastreams).
- [x] Raw-JSON intermediates committed for reproducibility.
- [x] J2534 + ELM327 transport (C#) present in `/src/PassThru`.
- [ ] This spec (`docs/`) — **you are here**.
- [ ] `opengen-etl verify` as a standalone CI check over the committed DBs.

**Deliverable:** a documented, reproducible dataset anyone can build tooling against.

## Phase 1 — Read-only MVP 🎯 next

Goal: open a car, see real faults and live data, on hardware people already have.

- [ ] `OpenDEN.Transport`: wrap existing J2534/ELM327 behind `ITransport` + capabilities ([04](04-transport-hal.md)).
- [ ] `OpenDEN.Data`: pack loader, schema-version gate, `v_oem_resolved` resolver.
- [ ] `OpenDEN.Core`: UDS client for `0x10/0x19/0x22/0x3E`; DTC byte→J2012 decode + KB lookup.
- [ ] Datastream **formula engine** (sandboxed evaluator, [05 §3](05-protocol-services.md)).
- [ ] `OpenDEN.App` CLI: `scan`, `dtc read/clear`, `live <module>`, offline `browse`.
- [ ] SocketCAN backend for dev/CI on `vcan0`.

**Deliverable:** read DTCs + live data on BYD/Chery/GWM/Geely/MG with a J2534 or ELM327. Genuinely useful to a workshop on day one.

## Phase 2 — Safe routines + verification

Goal: run the safe maintenance routines, and build the trust pipeline that turns catalogued → verified.

- [ ] **Safety Gate** implementation ([07 §2](07-safety-security-legal.md)) — the choke point, with tests.
- [ ] `RoutineControl` (`0x31`) executor building requests from `routine_param` ([05 §4](05-protocol-services.md)).
- [ ] Session discipline: lazy escalation, auto tester-present, `0x85` DTC-setting control around actuator tests.
- [ ] Trace capture + PII stripping + the `verified=1` promotion flow ([03 §4](03-provenance-trust-coverage.md)).
- [ ] Reference GUI (coverage matrix, DTC/live views, guided routine runner).
- [ ] First batch of community-verified routines (`OILRESET`, `EPB`, `THROTTLE`, `BMS`, `STEERING`).

**Deliverable:** do a real service (oil reset, brake bleed/EPB, throttle relearn) and prove it on a bus, with every routine's trust state visible.

## Phase 3 — Coverage depth + pack ecosystem

Goal: breadth and quality of data, and a sane way to distribute it.

- [ ] Pack registry + "updates available" + pack signing ([06 §5](06-oem-pack-model.md)).
- [ ] String-pool decoding campaign (turn handles into labels across makes, [08 §7](08-ingestion-pipeline.md)).
- [ ] VIN decode rules (`vin_rule`) → automatic model/year/engine resolution.
- [ ] Module CAN-ID mapping drive (fill `tx_id`/`rx_id` across common modules).
- [ ] Localization (EN first-class alongside source text).

**Deliverable:** deep, honest coverage with auto vehicle ID and easy pack updates.

## Phase 4 — Modern architectures (DoIP) + reach

Goal: the newer EV/zonal cars, and more places to run.

- [ ] `DoIpTransport` (ISO 13400) under the existing `ITransport` ([04 §2.4](04-transport-hal.md)).
- [ ] CAN-FD everywhere it matters.
- [ ] `RemotePassThru` relay productized (engine on laptop, interface on car/bench).
- [ ] Alternative engines welcomed: document the SQLite + ISO-TP contract so Python/Rust clients are first-class, not forks.

**Deliverable:** coverage of recent DoIP-based EVs and a networked bench workflow.

## Explicitly deferred / out of scope

| Item | Status | Why |
|---|---|---|
| ECU flashing / programming / tuning | Deferred, post-1.0, heavily gated | High brick/safety risk; OEM-specific; needs the full safety machinery first |
| SecurityAccess (0x27) key computation | Out of core; clean-room only, separate opt-in module, where lawful | Legal/IP; theft-adjacent — [07 §5](07-safety-security-legal.md) |
| Immobiliser / key learning / password read | **Never in core** | Theft-enabling — [07 §4](07-safety-security-legal.md) |
| Odometer / EEPROM write | **Never in core** | Fraud — [07 §4](07-safety-security-legal.md) |
| Cloud sync / telemetry | Not planned | Local-first by principle |

## North star

A workshop in Perth (or Lagos, or Lima) plugs a cheap interface into a 2024 BYD, reads real faults in plain language, does the scheduled service with verified routines, and trusts every number because the tool tells it exactly how much it knows — all without a subscription. The data's open, the engine's auditable, and the community keeps it current.

---

Previous: [09 — Contributing](09-contributing.md) · Back to [index](README.md)
