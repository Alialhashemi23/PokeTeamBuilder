import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Team } from '../../models/team';
import { TeamService } from '../../services/team';

@Component({
  selector: 'app-team-list',
  imports: [RouterLink, FormsModule],
  templateUrl: './team-list.html',
  styleUrl: './team-list.css',
})
export class TeamList implements OnInit {
  private teamService = inject(TeamService);

  readonly teams = signal<Team[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  newTeamName = '';

  ngOnInit(): void {
    this.loadTeams();
  }

  loadTeams(): void {
    this.loading.set(true);
    this.teamService.getAll().subscribe({
      next: (teams) => {
        this.teams.set(teams);
        this.loading.set(false);
        this.error.set(null);
      },
      error: (err) => {
        this.error.set(this.messageFrom(err));
        this.loading.set(false);
      },
    });
  }

  createTeam(): void {
    const name = this.newTeamName.trim();
    if (!name) {
      return;
    }

    this.teamService.create(name).subscribe({
      next: (team) => {
        this.teams.update((teams) => [...teams, team]);
        this.newTeamName = '';
        this.error.set(null);
      },
      error: (err) => this.error.set(this.messageFrom(err)),
    });
  }

  deleteTeam(team: Team): void {
    if (!confirm(`Delete team "${team.name}"?`)) {
      return;
    }

    this.teamService.delete(team.id).subscribe({
      next: () => {
        this.teams.update((teams) => teams.filter((t) => t.id !== team.id));
        this.error.set(null);
      },
      error: (err) => this.error.set(this.messageFrom(err)),
    });
  }

  private messageFrom(err: HttpErrorResponse): string {
    return typeof err.error === 'string' && err.error
      ? err.error
      : 'Something went wrong. Is the API running?';
  }
}
