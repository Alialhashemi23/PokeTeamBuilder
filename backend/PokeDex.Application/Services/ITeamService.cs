using PokeDex.Core.Models;

namespace PokeDex.Application.Services
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync(string ownerId);
        Task<Team?> GetTeamByIdAsync(int id, string ownerId);
        Task<TeamResult> CreateTeamAsync(string name, string ownerId);
        Task<TeamResult> RenameTeamAsync(int teamId, string name, string ownerId);
        Task<TeamResult> DeleteTeamAsync(int teamId, string ownerId);
        Task<TeamResult> AddPokemonAsync(int teamId, int pokemonId, string ownerId);
        Task<TeamResult> RemovePokemonAsync(int teamId, int teamPokemonId, string ownerId);
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
