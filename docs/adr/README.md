# Architecture Decision Records

ADRs capture the *why* behind the big, hard-to-reverse choices, so future contributors understand the reasoning instead of re-litigating it (or worse, silently undoing it).

Format: context → decision → consequences. An ADR is immutable once accepted; a changed decision gets a new ADR that supersedes the old one.

| ADR | Title | Status |
|---|---|---|
| [0001](0001-relational-normalized-db.md) | Relational normalized DB, one SQLite per make | Accepted |
| [0002](0002-provenance-first.md) | Provenance-first data model | Accepted |
| [0003](0003-transport-hal.md) | C# engine over a transport HAL; Python ETL | Accepted |
| [0004](0004-safety-gating.md) | Safety gating & clean-room security policy | Accepted |
