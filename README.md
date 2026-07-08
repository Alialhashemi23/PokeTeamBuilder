# 🧩 PokeTeamBuilder
*A full-stack Pokémon team-building app — ASP.NET Core API + Angular client.*

## 📌 Overview
PokeTeamBuilder helps players create, manage, and visualize Pokémon teams. It pulls Pokémon data from the public **PokeAPI** into a SQL Server database, exposes it through a clean-architecture REST API, and ships an Angular frontend for browsing Pokémon and building teams of up to six.

---

## 🚀 Features
- **User Accounts** — register/login with JWT auth; teams are private to their owner
- **Team Builder** — create named teams, add/remove Pokémon (max 6 per team, no duplicates), rename and delete teams
- **Official Artwork Sprites** — fetched from PokeAPI and shown throughout the UI
- **Zero-Touch Startup** — migrations apply and the database seeds itself on first run
- **Bulk Pokémon Fetching** from PokeAPI with concurrency limits; re-seed anytime via `POST /api/admin/seed`
- **Clean Architecture** — Core / Data / Application / API layers with repositories and services
- **Pokédex Browsing** — searchable card grid with type filters, plus per-Pokémon detail pages with stat bars
- **Team Analysis** — live type-coverage report (threats, unresisted types) and average stat bars in the builder
- **Angular 21 UI** — teams list and team-builder pages with type-colored badges and a searchable Pokémon picker
- **Tested** — xUnit tests for team rules (EF Core InMemory) and Vitest specs for the client services
- **Docker Support** — one `docker compose up --build` runs SQL Server + API + frontend

---

## 📂 Project Structure
```
backend/
  PokeDex.Core/          Domain models + PokeAPI client
  PokeDex.Data/          EF Core DbContext, migrations, repositories
  PokeDex.Application/   Business services (seeding, team rules)
  PokeDex.API/           Controllers, DTOs, Swagger
  PokeDex.Tests/         xUnit tests
pokedex-client/          Angular frontend
```

---

## 🔌 API Endpoints

### Pokémon
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/pokemon` | All Pokémon, ordered by Pokédex number |
| GET | `/api/pokemon/{id}` | Pokémon by database ID |
| GET | `/api/pokemon/pokedex/{number}` | Pokémon by Pokédex number |

### Auth
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/auth/register` | Create an account `{ "email": "...", "password": "..." }` → JWT |
| POST | `/api/auth/login` | Sign in → JWT |

### Teams *(require `Authorization: Bearer <token>`; scoped to the signed-in user)*
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/teams` | All teams with members |
| GET | `/api/teams/{id}` | Single team |
| GET | `/api/teams/{id}/analysis` | Type coverage + stat analysis |
| POST | `/api/teams` | Create a team `{ "name": "..." }` |
| PUT | `/api/teams/{id}` | Rename a team |
| DELETE | `/api/teams/{id}` | Delete a team |
| POST | `/api/teams/{id}/pokemon` | Add a Pokémon `{ "pokemonId": 25 }` (max 6, no duplicates) |
| DELETE | `/api/teams/{id}/pokemon/{teamPokemonId}` | Remove a member slot |

### Types
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/types/effectiveness` | Full 18×18 type effectiveness chart |

### Admin
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/admin/seed?count=151` | Fetch Pokémon from PokeAPI into the database |

---

## 🏃 Running the App

### 🐳 Docker (easiest)
```bash
docker compose up --build
```
That's it — SQL Server, the API, and the frontend all start together; migrations
apply and Gen 1 seeds itself on first run. Open **http://localhost:8080**
(Swagger: http://localhost:5080). Data persists in a named volume across
restarts. Override the database password with `SA_PASSWORD=... docker compose up`.

> Apple Silicon note: the SQL Server image is amd64-only — enable Rosetta
> emulation in Docker Desktop settings.

### 🛠️ Local development

**Backend** (requires .NET 9 SDK + SQL Server / LocalDB):
```bash
cd backend
dotnet run --project PokeDex.API
```
On first run this applies migrations and seeds Gen 1 from PokeAPI automatically.
To re-seed or fetch more Pokémon later: `POST /api/admin/seed?count=251` (Swagger is served at the root in development).

**Frontend** (requires Node 20+):
```bash
cd pokedex-client
npm install
npm start   # proxies /api to the backend (see proxy.conf.json)
```
Open http://localhost:4200.

**Tests:**
```bash
cd backend && dotnet test          # backend
cd pokedex-client && npm test      # frontend
```

---

## 🗺️ Roadmap
Milestones are documented feature-by-feature in **[ROADMAP.md](ROADMAP.md)**:
- ✅ **M0 — Core MVP**: ingestion, Pokémon + Teams API, team builder UI
- ✅ **M1 — Make It Feel Real**: sprites, zero-touch startup, template cleanup
- ✅ **M2 — Pokédex Browsing**: searchable/filterable Pokédex + Pokémon detail pages
- ✅ **M3 — Accounts & Ownership**: JWT auth, per-user teams
- ✅ **M4 — Team Analysis**: type coverage, threats, stat summaries
- 📋 **M5 — Deployment**: containerization ✅; hosting + CI remain
- 💭 **Backlog — Full Builder**: movesets, abilities, natures, EVs/IVs, sharing
