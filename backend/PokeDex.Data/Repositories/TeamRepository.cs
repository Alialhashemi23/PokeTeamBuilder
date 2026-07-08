using Microsoft.EntityFrameworkCore;
using PokeDex.Core.Models;

namespace PokeDex.Data.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly PokedexDbContext _context;

        public TeamRepository(PokedexDbContext context)
        {
            _context = context;
        }

        public async Task<List<Team>> GetAllByOwnerAsync(string ownerId)
        {
            return await _context.Teams
                .Where(t => t.OwnerId == ownerId)
                .Include(t => t.TeamPokemon)
                    .ThenInclude(tp => tp.Pokemon)
                        .ThenInclude(p => p.PrimaryType)
                .Include(t => t.TeamPokemon)
                    .ThenInclude(tp => tp.Pokemon)
                        .ThenInclude(p => p.SecondaryType)
                .OrderBy(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.TeamPokemon)
                    .ThenInclude(tp => tp.Pokemon)
                        .ThenInclude(p => p.PrimaryType)
                .Include(t => t.TeamPokemon)
                    .ThenInclude(tp => tp.Pokemon)
                        .ThenInclude(p => p.SecondaryType)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team> AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            return team;
        }

        public void Remove(Team team)
        {
            _context.Teams.Remove(team);
        }

        public void RemoveMember(TeamPokemon teamPokemon)
        {
            _context.TeamPokemon.Remove(teamPokemon);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
