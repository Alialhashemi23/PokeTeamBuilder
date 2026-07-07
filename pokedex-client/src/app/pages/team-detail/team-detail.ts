import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Team, TeamMember } from '../../models/team';
import { Pokemon } from '../../models/pokemon';
import { TeamService } from '../../services/team';
import { Pokemon as PokemonApi } from '../../services/pokemon';

export const TEAM_SIZE = 6;

@Component({
  selector: 'app-team-detail',
  imports: [RouterLink, FormsModule],
  templateUrl: './team-detail.html',
  styleUrl: './team-detail.css',
})
export class TeamDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private teamService = inject(TeamService);
  private pokemonApi = inject(PokemonApi);

  readonly team = signal<Team | null>(null);
  readonly allPokemon = signal<Pokemon[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly search = signal('');
  readonly renaming = signal(false);
  renameValue = '';

  /** Filled member slots padded with nulls up to the fixed team size */
  readonly slots = computed<(TeamMember | null)[]>(() => {
    const members: (TeamMember | null)[] = [...(this.team()?.members ?? [])];
    while (members.length < TEAM_SIZE) {
      members.push(null);
    }
    return members;
  });

  readonly isFull = computed(() => (this.team()?.members.length ?? 0) >= TEAM_SIZE);

  readonly memberPokemonIds = computed(
    () => new Set(this.team()?.members.map((m) => m.pokemon.id) ?? [])
  );

  readonly filteredPokemon = computed(() => {
    const query = this.search().trim().toLowerCase();
    const pokemon = this.allPokemon();
    if (!query) {
      return pokemon;
    }
    return pokemon.filter(
      (p) =>
        p.name.toLowerCase().includes(query) ||
        p.pokedexNumber.toString() === query ||
        p.primaryType.name.toLowerCase() === query ||
        p.secondaryType?.name.toLowerCase() === query
    );
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.teamService.getById(id).subscribe({
      next: (team) => {
        this.team.set(team);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(this.messageFrom(err));
        this.loading.set(false);
      },
    });

    this.pokemonApi.getAll().subscribe({
      next: (pokemon) => this.allPokemon.set(pokemon),
      error: () =>
        this.error.set('Could not load the Pokédex. Has the database been seeded? (POST /api/admin/seed)'),
    });
  }

  addPokemon(pokemon: Pokemon): void {
    const team = this.team();
    if (!team || this.isFull() || this.memberPokemonIds().has(pokemon.id)) {
      return;
    }

    this.teamService.addPokemon(team.id, pokemon.id).subscribe({
      next: (updated) => {
        this.team.set(updated);
        this.error.set(null);
      },
      error: (err) => this.error.set(this.messageFrom(err)),
    });
  }

  removeMember(member: TeamMember): void {
    const team = this.team();
    if (!team) {
      return;
    }

    this.teamService.removePokemon(team.id, member.teamPokemonId).subscribe({
      next: (updated) => {
        this.team.set(updated);
        this.error.set(null);
      },
      error: (err) => this.error.set(this.messageFrom(err)),
    });
  }

  startRename(): void {
    this.renameValue = this.team()?.name ?? '';
    this.renaming.set(true);
  }

  saveRename(): void {
    const team = this.team();
    const name = this.renameValue.trim();
    if (!team || !name) {
      this.renaming.set(false);
      return;
    }

    this.teamService.rename(team.id, name).subscribe({
      next: (updated) => {
        this.team.set(updated);
        this.renaming.set(false);
        this.error.set(null);
      },
      error: (err) => this.error.set(this.messageFrom(err)),
    });
  }

  cancelRename(): void {
    this.renaming.set(false);
  }

  private messageFrom(err: HttpErrorResponse): string {
    return typeof err.error === 'string' && err.error
      ? err.error
      : 'Something went wrong. Is the API running?';
  }
}
