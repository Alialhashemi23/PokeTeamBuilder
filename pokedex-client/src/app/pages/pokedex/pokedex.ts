import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Pokemon } from '../../models/pokemon';
import { Pokemon as PokemonApi } from '../../services/pokemon';

@Component({
  selector: 'app-pokedex',
  imports: [RouterLink, FormsModule],
  templateUrl: './pokedex.html',
  styleUrl: './pokedex.css',
})
export class Pokedex implements OnInit {
  private pokemonApi = inject(PokemonApi);

  readonly allPokemon = signal<Pokemon[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly search = signal('');
  readonly typeFilter = signal<string | null>(null);

  /** Distinct type names present in the loaded data, in Pokédex type order */
  readonly types = computed(() => {
    const seen = new Map<number, string>();
    for (const pokemon of this.allPokemon()) {
      seen.set(pokemon.primaryType.id, pokemon.primaryType.name);
      if (pokemon.secondaryType) {
        seen.set(pokemon.secondaryType.id, pokemon.secondaryType.name);
      }
    }
    return [...seen.entries()].sort(([a], [b]) => a - b).map(([, name]) => name);
  });

  readonly filtered = computed(() => {
    const query = this.search().trim().toLowerCase();
    const type = this.typeFilter();
    return this.allPokemon().filter((p) => {
      const matchesQuery =
        !query || p.name.toLowerCase().includes(query) || p.pokedexNumber.toString() === query;
      const matchesType =
        !type || p.primaryType.name === type || p.secondaryType?.name === type;
      return matchesQuery && matchesType;
    });
  });

  ngOnInit(): void {
    this.pokemonApi.getAll().subscribe({
      next: (pokemon) => {
        this.allPokemon.set(pokemon);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load the Pokédex. Is the API running?');
        this.loading.set(false);
      },
    });
  }

  toggleType(type: string): void {
    this.typeFilter.update((current) => (current === type ? null : type));
  }
}
