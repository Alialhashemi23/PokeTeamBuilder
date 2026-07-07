# 🗺️ PokeTeamBuilder Roadmap

Milestones are ordered by dependency, not just priority — M3 (accounts) intentionally
comes before deeper feature investment because ownership reshapes the data model and
every endpoint after it.

Status: ✅ shipped · 🔨 next up · 📋 planned · 💭 backlog

---

## ✅ M0 — Core MVP *(shipped)*

The end-to-end loop: ingest Pokémon data, expose it over REST, build teams in a UI.

| Feature | Notes |
|---------|-------|
| PokeAPI ingestion | Concurrent fetching (capped at 10), Gen 1 by default via `POST /api/admin/seed` |
| Pokémon REST API | List, by ID, by Pokédex number |
| Teams API | CRUD + add/remove members; max 6, no duplicates, name validation — enforced server-side |
| Team builder UI | Teams list, 6-slot builder page with stats, searchable picker, inline rename |
| Test baseline | 15 xUnit tests (team rules), 7 Vitest specs (client services) |

---

## ✅ M1 — Make It Feel Real *(shipped)*

Cheap, high-impact polish. Goal: someone can clone the repo and be building teams in
two commands, and the app looks like a Pokémon app.

### 1.1 Pokémon sprites ✅
Official-artwork sprite URLs are stored during ingestion (`SpriteUrl` column +
migration) and rendered in picker rows, team slots, and team cards. Re-seeding
backfills sprites onto rows seeded before sprites were tracked.

### 1.2 Zero-touch startup ✅
Migrations apply on startup and an empty database seeds itself from PokeAPI;
failures are logged without blocking startup. `POST /api/admin/seed` remains for
re-seeding / fetching more generations.

### 1.3 Template cleanup ✅
`WeatherForecastController` and `WeatherForecast.cs` removed.

---

## ✅ M2 — Pokédex Browsing *(shipped)*

A standalone Pokédex page, independent of team building.

### 2.1 Pokédex page ✅
`/pokedex` in the nav: card grid with sprites and type badges, instant client-side
search (name/number) and toggleable type-color filter chips.

### 2.2 Pokémon detail view ✅
`/pokedex/:id`: large artwork, stat bars scaled to the 255 base-stat cap with a
total, and links to the teams the Pokémon is on.

### 2.3 More generations ✅*
`POST /api/admin/seed?count=` accepts up to 1000 and the Pokédex filters
client-side, which stays snappy at that scale. *Not yet validated against a live
database with 386+ rows — do one big seed and confirm.

---

## 📋 M3 — Accounts & Ownership *(decision gate)*

The biggest architectural fork: today every team belongs to everyone. Decide and
implement before investing further, because it touches every table and endpoint.

### 3.1 Authentication
Sign up / log in (ASP.NET Core Identity + JWT, or an external provider).
- **Done when:** the API issues and validates tokens; the Angular app has
  login/logout state and an HTTP interceptor.

### 3.2 Team ownership
Teams belong to a user; all team endpoints scope to the caller.
- Migration adds `UserId` to `Team`; authorization enforced server-side.
- **Done when:** two users can't see or modify each other's teams.

*Explicit alternative:* stay single-user (a personal/local tool) and skip M3 —
that's a valid product decision, but it should be made deliberately.

---

## 📋 M4 — Team Analysis

The fun differentiator. Stacks cleanly on top of any of the above.

### 4.1 Type effectiveness data
Store the 18×18 type matchup chart (seeded statically or from PokeAPI).
- **Done when:** an endpoint returns effectiveness for any attacking/defending pair.

### 4.2 Team coverage report
Per team: defensive weaknesses/resistances (which attacking types hit 2×/4×, what
you resist) and a summary of gaps.
- **Done when:** the team builder page shows a coverage panel that updates as
  Pokémon are added/removed.

### 4.3 Stat overview
Team-wide stat comparison (e.g., speed tiers, average bulk) to spot imbalances.
- **Done when:** the builder shows a compact stats summary per team.

---

## 📋 M5 — Deployment

Take it off localhost.

### 5.1 Containerization
Dockerfiles for API + client, docker-compose with SQL Server (or a switch to
PostgreSQL/SQLite — decide here).
- **Done when:** `docker compose up` serves the whole app locally.

### 5.2 Hosting & config
Environment-based connection strings and CORS, HTTPS, CI pipeline
(build + test on push).
- **Done when:** a pushed commit is automatically built and tested; the app runs
  on a real URL.

---

## 💭 Backlog — Full Builder

Deliberately deferred: each of these significantly expands the data model and
PokeAPI ingestion.

- **Movesets** — pick up to 4 moves per team slot (needs move ingestion + per-slot config)
- **Abilities & natures** — per-slot choices affecting stats
- **EVs/IVs** — stat customization with calculated final stats
- **Team sharing** — public read-only links or export formats (e.g., Showdown paste)
- **Offensive coverage** — analysis based on chosen moves, not just typing (depends on movesets)
