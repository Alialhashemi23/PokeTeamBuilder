import { Routes } from '@angular/router';
import { TeamList } from './pages/team-list/team-list';
import { TeamDetail } from './pages/team-detail/team-detail';

export const routes: Routes = [
  { path: '', redirectTo: 'teams', pathMatch: 'full' },
  { path: 'teams', component: TeamList, title: 'PokeTeamBuilder — Teams' },
  { path: 'teams/:id', component: TeamDetail, title: 'PokeTeamBuilder — Team' },
  { path: '**', redirectTo: 'teams' },
];
