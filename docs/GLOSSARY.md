# Glossary

Domain terms and acronyms used across this spec.

## Protocols & standards

| Term | Meaning |
|---|---|
| **OBD-II** | On-Board Diagnostics II — the legally-mandated emissions diagnostic interface and connector. |
| **CAN / CAN-FD** | Controller Area Network (ISO 11898); the vehicle bus. CAN-FD adds flexible data rate and larger payloads. |
| **ISO-TP** | ISO 15765-2 transport protocol — segments diagnostic messages larger than one CAN frame, with flow control. |
| **UDS** | Unified Diagnostic Services (ISO 14229) — the modern diagnostic service layer. The primary target of this project. |
| **KWP2000** | Keyword Protocol 2000 (ISO 14230) — the pre-UDS service layer, still on older modules. |
| **DoIP** | Diagnostics over IP (ISO 13400) — UDS over automotive ethernet, used by newer EV/zonal cars. |
| **J2534 / PassThru** | SAE J2534 — a standard API for a PC to talk to a vehicle through a vendor-neutral interface device. |
| **ELM327** | A cheap, ubiquitous OBD interface chip/clone; fine for reads, weak on multi-frame. |
| **SocketCAN** | The Linux kernel CAN subsystem; `vcan0` is its virtual bus (used for CI). |
| **J2012** | SAE J2012 — the DTC format standard (`P`/`C`/`B`/`U` + digits). |
| **GB/T** | Chinese national ("Guóbiāo") standards; relevant ones include EV charging comms and telematics. |

## Diagnostic concepts

| Term | Meaning |
|---|---|
| **DTC** | Diagnostic Trouble Code — a fault code (e.g. `P020400`). Read with UDS `0x19`, cleared with `0x14`. |
| **Datastream / PID / DID** | Live data. PID = Parameter ID (legacy/OBD); DID = Data Identifier (UDS `0x22`). |
| **Routine** | A service procedure run via UDS `0x31` RoutineControl (or IO control), e.g. an oil-service reset. |
| **Module / ECU** | An electronic control unit on the bus (ECM, TCM, ABS, SRS, BCM, BMS, …). |
| **Actuator test** | Commanding an output (UDS `0x2F`) to confirm a component works. |
| **SecurityAccess** | UDS `0x27` seed/key challenge that unlocks protected services. **Gated** in this project. |
| **Tester-present** | UDS `0x3E` keep-alive that stops a session timing out. |
| **Adaptation / Coding** | Writing configuration to a module (UDS `0x2E`) — e.g. after a part swap. Gated/confirmed. |
| **Immobiliser (IMMO)** | Anti-theft system binding keys to the car. Modelled as metadata only; **not executable** here. |

## Project-specific terms

| Term | Meaning |
|---|---|
| **OpenDEN** | Working codename for the diagnostic engine/app this spec describes (*Open Diagnostic ENgine*). |
| **Opium-Den-OEM** | This repository — the data dump and community bootstrapper. |
| **Pack** | One make's shippable knowledge: `diag_<MAKE>.sqlite` + manifest + traces ([06](06-oem-pack-model.md)). |
| **Engine (OEM role)** | An `oem` row with real diagnostic logic, as opposed to an **alias** (a badge reusing an engine). |
| **Alias** | A badge/brand that resolves to another engine via `oem.alias_of` (e.g. EXEED → CHERY). |
| **Provenance** | Where a datum came from + how much to trust it ([03](03-provenance-trust-coverage.md)). |
| **`frame_source`** | Origin of a routine's request template; part of the trust spine. |
| **`verified`** | Whether a routine has a logged bus trace proving it works. Never set by ingestion. |
| **Formula engine** | The sandboxed evaluator for the datastream scaling DSL ([05 §3](05-protocol-services.md)). |
| **Safety Gate** | The single L3 choke point every bus write/routine must pass ([07 §2](07-safety-security-legal.md)). |
| **Restraints guard** | The `module.is_restraints` flag + policy protecting airbag/SRS modules. |
| **Gated function** | A theft/fraud-adjacent function class catalogued as metadata but not shipped executable. |
| **Clean-room** | An independent re-implementation that doesn't copy vendor code; the only lawful path for any `0x27` work. |
| **ETL** | Extract-Transform-Load — the raw-dump → normalized-DB pipeline ([08](08-ingestion-pipeline.md)). |
| **`etl_run`** | An audit row for one ingestion pass; every content row links to one. |
| **String pool** | The source tool's table of text handles that DTC/datastream rows reference; much of it awaits decode. |

## Module acronyms (in the schema)

`ECM` engine, `TCM` transmission, `ABS` brakes, `SRS` airbags (restraints), `BCM` body, `BMS` battery mgmt, `EPS` power steering, `IC` instrument cluster, `HVAC` climate, `ADAS` driver assist, `IMMO` immobiliser, `INFORMA` infotainment, `LED` lighting, `ALL_SYSTEM` whole-vehicle scan.
