import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pokemon as PokemonModel } from '../models/pokemon';

@Injectable({
  providedIn: 'root',
})
export class Pokemon {
  private apiUrl = '/api/pokemon';

  constructor(private http: HttpClient) {}

  getAll(): Observable<PokemonModel[]> {
    return this.http.get<PokemonModel[]>(this.apiUrl);
  }

  getById(id: number): Observable<PokemonModel> {
    return this.http.get<PokemonModel>(`${this.apiUrl}/${id}`);
  }

  getByPokedexNumber(pokedexNumber: number): Observable<PokemonModel> {
    return this.http.get<PokemonModel>(`${this.apiUrl}/pokedex/${pokedexNumber}`);
  }
}
