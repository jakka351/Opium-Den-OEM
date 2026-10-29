# ADR-0004 — Safety gating & clean-room security policy

**Status:** Accepted · **Date:** 2026-10

## Context

A capable diagnostic tool is one keystroke away from being a theft tool, a fraud tool, or a way to hurt someone. The source data *contains* immobiliser, key, password-read, odometer, EEPROM-write and SecurityAccess material. The project has to decide — structurally, not just in a README — what it will and won't do with that. Getting this wrong makes the project indefensible, gets it taken down, and betrays the workshops it's meant to serve.

See [07 — Safety, Security & Legal](../07-safety-security-legal.md) for the full policy; this ADR records the decision and its rationale.

## Decision

1. **Single Safety Gate.** Every bus write/routine passes exactly one choke point at the L3 boundary. No bypass path is permitted in the codebase. Nothing reaches the bus as a write without it.

2. **Restraints guard.** `module.is_restraints` gates airbag/SRS modules: reads allowed, writes/actuator-tests denied or behind explicit hazard confirmation + capable backend + trace. Adding a restraints module without the flag is a rejectable PR.

3. **Gated functions are metadata, not executables.** IMMO, SMART_KEY, PASSWORD_READING, RFID, ODOMETER/ODO write, EEPROM write, ECUCLONING, ECUFLASHING are **catalogued** (so a tech can see they exist) but **never shipped as executable sequences**. The engine refuses them with a clear message.

4. **Clean-room-only security.** The schema stores the *structure* of SecurityAccess (`seedkey`, `native_algo`) so the engine can recognise and cleanly refuse a lock it can't open. It stores **no working key algorithms, no vendor binaries, no lifted constants**. Any `0x27` implementation, if ever built, is an independent clean-room effort in a **separate, opt-in, clearly-licensed module**, developed where lawful — never in core, never default.

5. **No binaries, ever.** `.so`/`.p12`/calibration files are referenced by filename/hash only (`GATED-LEGAL/IP`), never stored.

## Consequences

**Positive**
- The project is a *diagnostic* tool, defensibly — not a circumvention device.
- One auditable choke point; safety reviewers have one place to look.
- The honest middle path: transparent about what functions *exist* (information) while refusing to be the thing that *executes* theft/fraud (tooling).
- Legal exposure (IP, anti-circumvention) is structurally minimised.

**Negative / costs**
- Some users will want the gated functions. That's a deliberate "no" — the cost of staying legitimate and useful to the actual target users.
- Contributors must learn the red lines ([07 §7](../07-safety-security-legal.md)); some well-meaning PRs get declined on policy, not merit.
- Clean-room discipline is slow and must be done right or not at all.

## Notes

This ADR is load-bearing for the project's existence. A change that weakens the gate, ships a gated function executable, or bundles key material into core doesn't just need a new ADR — it needs a hard look at whether it's still the same project. The answer, for the people this is built for, is no.
