import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Team } from '../models/team';
import { TeamAnalysis } from '../models/analysis';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private apiUrl = '/api/teams';
  private http = inject(HttpClient);

  getAll(): Observable<Team[]> {
    return this.http.get<Team[]>(this.apiUrl);
  }

  getById(id: number): Observable<Team> {
    return this.http.get<Team>(`${this.apiUrl}/${id}`);
  }

  create(name: string): Observable<Team> {
    return this.http.post<Team>(this.apiUrl, { name });
  }

  rename(id: number, name: string): Observable<Team> {
    return this.http.put<Team>(`${this.apiUrl}/${id}`, { name });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addPokemon(teamId: number, pokemonId: number): Observable<Team> {
    return this.http.post<Team>(`${this.apiUrl}/${teamId}/pokemon`, { pokemonId });
  }

  removePokemon(teamId: number, teamPokemonId: number): Observable<Team> {
    return this.http.delete<Team>(`${this.apiUrl}/${teamId}/pokemon/${teamPokemonId}`);
  }

  getAnalysis(teamId: number): Observable<TeamAnalysis> {
    return this.http.get<TeamAnalysis>(`${this.apiUrl}/${teamId}/analysis`);
  }
}
