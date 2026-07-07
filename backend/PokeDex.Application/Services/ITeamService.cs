using PokeDex.Core.Models;

namespace PokeDex.Application.Services
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<TeamResult> CreateTeamAsync(string name);
        Task<TeamResult> RenameTeamAsync(int teamId, string name);
        Task<TeamResult> DeleteTeamAsync(int teamId);
        Task<TeamResult> AddPokemonAsync(int teamId, int pokemonId);
        Task<TeamResult> RemovePokemonAsync(int teamId, int teamPokemonId);
    }

    public enum TeamError
    {
        None,
        InvalidName,
        TeamNotFound,
        PokemonNotFound,
        MemberNotFound,
        TeamFull,
        DuplicatePokemon
    }

    public class TeamResult
    {
        public bool Success { get; private set; }
        public TeamError Error { get; private set; }
        public string? Message { get; private set; }
        public Team? Team { get; private set; }

        public static TeamResult Ok(Team? team = null) =>
            new() { Success = true, Error = TeamError.None, Team = team };

        public static TeamResult Fail(TeamError error, string message) =>
            new() { Success = false, Error = error, Message = message };
    }
}
