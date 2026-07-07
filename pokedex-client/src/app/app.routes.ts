import { Routes } from '@angular/router';
import { TeamList } from './pages/team-list/team-list';
import { TeamDetail } from './pages/team-detail/team-detail';
import { Pokedex } from './pages/pokedex/pokedex';
import { PokemonDetail } from './pages/pokemon-detail/pokemon-detail';

export const routes: Routes = [
  { path: '', redirectTo: 'teams', pathMatch: 'full' },
  { path: 'teams', component: TeamList, title: 'PokeTeamBuilder — Teams' },
  { path: 'teams/:id', component: TeamDetail, title: 'PokeTeamBuilder — Team' },
  { path: 'pokedex', component: Pokedex, title: 'PokeTeamBuilder — Pokédex' },
  { path: 'pokedex/:id', component: PokemonDetail, title: 'PokeTeamBuilder — Pokémon' },
  { path: '**', redirectTo: 'teams' },
];
