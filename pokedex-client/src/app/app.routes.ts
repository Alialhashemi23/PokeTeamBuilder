import { Routes } from '@angular/router';
import { TeamList } from './pages/team-list/team-list';
import { TeamDetail } from './pages/team-detail/team-detail';
import { Pokedex } from './pages/pokedex/pokedex';
import { PokemonDetail } from './pages/pokemon-detail/pokemon-detail';
import { Login } from './pages/login/login';
import { authGuard } from './auth/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'teams', pathMatch: 'full' },
  { path: 'login', component: Login, title: 'PokeTeamBuilder — Sign In' },
  { path: 'teams', component: TeamList, canActivate: [authGuard], title: 'PokeTeamBuilder — Teams' },
  { path: 'teams/:id', component: TeamDetail, canActivate: [authGuard], title: 'PokeTeamBuilder — Team' },
  { path: 'pokedex', component: Pokedex, title: 'PokeTeamBuilder — Pokédex' },
  { path: 'pokedex/:id', component: PokemonDetail, title: 'PokeTeamBuilder — Pokémon' },
  { path: '**', redirectTo: 'teams' },
];
