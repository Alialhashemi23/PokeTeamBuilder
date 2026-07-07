import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Pokemon } from '../../models/pokemon';
import { Team } from '../../models/team';
import { Pokemon as PokemonApi } from '../../services/pokemon';
import { TeamService } from '../../services/team';

/** Highest base stat in the games; used to scale the stat bars */
const MAX_BASE_STAT = 255;

interface StatRow {
  label: string;
  value: number;
  percent: number;
}

@Component({
  selector: 'app-pokemon-detail',
  imports: [RouterLink],
  templateUrl: './pokemon-detail.html',
  styleUrl: './pokemon-detail.css',
})
export class PokemonDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private pokemonApi = inject(PokemonApi);
  private teamService = inject(TeamService);

  readonly pokemon = signal<Pokemon | null>(null);
  readonly teams = signal<Team[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly stats = computed<StatRow[]>(() => {
    const p = this.pokemon();
    if (!p) {
      return [];
    }
    const rows = [
      { label: 'HP', value: p.hp },
      { label: 'Attack', value: p.attack },
      { label: 'Defense', value: p.defense },
      { label: 'Sp. Atk', value: p.specialAttack },
      { label: 'Sp. Def', value: p.specialDefense },
      { label: 'Speed', value: p.speed },
    ];
    return rows.map((row) => ({ ...row, percent: (row.value / MAX_BASE_STAT) * 100 }));
  });

  readonly statTotal = computed(() =>
    this.stats().reduce((total, row) => total + row.value, 0)
  );

  /** Teams this Pokémon is a member of */
  readonly onTeams = computed(() => {
    const p = this.pokemon();
    if (!p) {
      return [];
    }
    return this.teams().filter((team) =>
      team.members.some((member) => member.pokemon.id === p.id)
    );
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.pokemonApi.getById(id).subscribe({
      next: (pokemon) => {
        this.pokemon.set(pokemon);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(`Pokémon not found.`);
        this.loading.set(false);
      },
    });

    this.teamService.getAll().subscribe({
      next: (teams) => this.teams.set(teams),
      error: () => {
        // Teams are supplementary here; the page still works without them
      },
    });
  }
}
