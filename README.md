# 🧩 PokeTeamBuilder
*A full-stack Pokémon team-building app — ASP.NET Core API + Angular client.*

## 📌 Overview
PokeTeamBuilder helps players create, manage, and visualize Pokémon teams. It pulls Pokémon data from the public **PokeAPI** into a SQL Server database, exposes it through a clean-architecture REST API, and ships an Angular frontend for browsing Pokémon and building teams of up to six.

---

## 🚀 Features
- **Team Builder** — create named teams, add/remove Pokémon (max 6 per team, no duplicates), rename and delete teams
- **Bulk Pokémon Fetching** from PokeAPI with concurrency limits
- **Database Seeding** endpoint (`POST /api/admin/seed`) — Gen 1 by default
- **Clean Architecture** — Core / Data / Application / API layers with repositories and services
- **Angular 21 UI** — teams list and team-builder pages with type-colored badges and a searchable Pokémon picker
- **Tested** — xUnit tests for team rules (EF Core InMemory) and Vitest specs for the client services

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

### Teams
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/teams` | All teams with members |
| GET | `/api/teams/{id}` | Single team |
| POST | `/api/teams` | Create a team `{ "name": "..." }` |
| PUT | `/api/teams/{id}` | Rename a team |
| DELETE | `/api/teams/{id}` | Delete a team |
| POST | `/api/teams/{id}/pokemon` | Add a Pokémon `{ "pokemonId": 25 }` (max 6, no duplicates) |
| DELETE | `/api/teams/{id}/pokemon/{teamPokemonId}` | Remove a member slot |

### Admin
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/admin/seed?count=151` | Fetch Pokémon from PokeAPI into the database |

---

## 🏃 Running the App

**Backend** (requires .NET 9 SDK + SQL Server / LocalDB):
```bash
cd backend
dotnet ef database update --project PokeDex.Data --startup-project PokeDex.API
dotnet run --project PokeDex.API
```
Then seed the database once via Swagger (served at the root in development) or:
```bash
curl -X POST "https://localhost:7057/api/admin/seed?count=151"
```

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
- [x] Pokémon ingestion from PokeAPI + seeding
- [x] Pokémon REST endpoints
- [x] Teams CRUD + membership rules (max 6, no duplicates)
- [x] Angular teams list + team-builder UI
- [ ] Pokédex browsing page (search/filter by type)
- [ ] Team type-coverage analysis (weaknesses/resistances)
- [ ] Pokémon sprites in the UI
- [ ] Movesets, abilities, and natures per team slot
