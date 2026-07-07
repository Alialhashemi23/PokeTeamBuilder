using Microsoft.Extensions.Logging;
using PokeDex.Core.Models;
using PokeDex.Data.Repositories;

namespace PokeDex.Application.Services
{
    public class TeamService : ITeamService
    {
        public const int MaxTeamSize = 6;
        public const int MaxNameLength = 50;

        private readonly ITeamRepository _teamRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ILogger<TeamService> _logger;

        public TeamService(
            ITeamRepository teamRepository,
            IPokemonRepository pokemonRepository,
            ILogger<TeamService> logger)
        {
            _teamRepository = teamRepository;
            _pokemonRepository = pokemonRepository;
            _logger = logger;
        }

        public Task<List<Team>> GetAllTeamsAsync() => _teamRepository.GetAllAsync();

        public Task<Team?> GetTeamByIdAsync(int id) => _teamRepository.GetByIdAsync(id);

        public async Task<TeamResult> CreateTeamAsync(string name)
        {
            var nameError = ValidateName(name);
            if (nameError != null)
            {
                return nameError;
            }

            var team = new Team
            {
                Name = name.Trim(),
                CreatedDate = DateTime.UtcNow
            };

            await _teamRepository.AddAsync(team);
            await _teamRepository.SaveChangesAsync();

            _logger.LogInformation("Created team {TeamId} ({TeamName})", team.Id, team.Name);
            return TeamResult.Ok(team);
        }

        public async Task<TeamResult> RenameTeamAsync(int teamId, string name)
        {
            var nameError = ValidateName(name);
            if (nameError != null)
            {
                return nameError;
            }

            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                return TeamResult.Fail(TeamError.TeamNotFound, $"Team with ID {teamId} not found");
            }

            team.Name = name.Trim();
            await _teamRepository.SaveChangesAsync();

            return TeamResult.Ok(team);
        }

        public async Task<TeamResult> DeleteTeamAsync(int teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                return TeamResult.Fail(TeamError.TeamNotFound, $"Team with ID {teamId} not found");
            }

            _teamRepository.Remove(team);
            await _teamRepository.SaveChangesAsync();

            _logger.LogInformation("Deleted team {TeamId} ({TeamName})", team.Id, team.Name);
            return TeamResult.Ok();
        }

        public async Task<TeamResult> AddPokemonAsync(int teamId, int pokemonId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                return TeamResult.Fail(TeamError.TeamNotFound, $"Team with ID {teamId} not found");
            }

            if (team.TeamPokemon.Count >= MaxTeamSize)
            {
                return TeamResult.Fail(TeamError.TeamFull, $"Team already has the maximum of {MaxTeamSize} Pokemon");
            }

            if (team.TeamPokemon.Any(tp => tp.PokemonId == pokemonId))
            {
                return TeamResult.Fail(TeamError.DuplicatePokemon, "That Pokemon is already on this team");
            }

            var pokemon = await _pokemonRepository.GetByIdAsync(pokemonId);
            if (pokemon == null)
            {
                return TeamResult.Fail(TeamError.PokemonNotFound, $"Pokemon with ID {pokemonId} not found");
            }

            team.TeamPokemon.Add(new TeamPokemon
            {
                TeamId = team.Id,
                PokemonId = pokemon.Id
            });
            await _teamRepository.SaveChangesAsync();

            // Reload so the new member comes back with its Pokemon navigation populated
            var updated = await _teamRepository.GetByIdAsync(teamId);
            return TeamResult.Ok(updated);
        }

        public async Task<TeamResult> RemovePokemonAsync(int teamId, int teamPokemonId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                return TeamResult.Fail(TeamError.TeamNotFound, $"Team with ID {teamId} not found");
            }

            var member = team.TeamPokemon.FirstOrDefault(tp => tp.Id == teamPokemonId);
            if (member == null)
            {
                return TeamResult.Fail(TeamError.MemberNotFound, $"Team member with ID {teamPokemonId} not found on team {teamId}");
            }

            _teamRepository.RemoveMember(member);
            await _teamRepository.SaveChangesAsync();

            return TeamResult.Ok(team);
        }

        private static TeamResult? ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return TeamResult.Fail(TeamError.InvalidName, "Team name is required");
            }

            if (name.Trim().Length > MaxNameLength)
            {
                return TeamResult.Fail(TeamError.InvalidName, $"Team name must be {MaxNameLength} characters or fewer");
            }

            return null;
        }
    }
}
