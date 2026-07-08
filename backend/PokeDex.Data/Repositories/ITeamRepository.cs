using PokeDex.Core.Models;

namespace PokeDex.Data.Repositories
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetAllByOwnerAsync(string ownerId);
        Task<Team?> GetByIdAsync(int id);
        Task<Team> AddAsync(Team team);
        void Remove(Team team);
        void RemoveMember(TeamPokemon teamPokemon);
        Task<int> SaveChangesAsync();
    }
}
