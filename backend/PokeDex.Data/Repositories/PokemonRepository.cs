using Microsoft.EntityFrameworkCore;
using PokeDex.Core.Models;

namespace PokeDex.Data.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly PokedexDbContext _context;

        public PokemonRepository(PokedexDbContext context)
        {
            _context = context;
        }

        public async Task<Pokemon?> GetByPokedexNumberAsync(int pokedexNumber)
        {
            return await _context.Pokemon
                .Include(p => p.PrimaryType)
                .Include(p => p.SecondaryType)
                .FirstOrDefaultAsync(p => p.PokedexNumber == pokedexNumber);
        }

        public async Task<List<Pokemon>> GetAllAsync()
        {
            return await _context.Pokemon
                .Include(p => p.PrimaryType)
                .Include(p => p.SecondaryType)
                .OrderBy(p => p.PokedexNumber)
                .ToListAsync();
        }

        public async Task<Pokemon> AddAsync(Pokemon pokemon)
        {
            await _context.Pokemon.AddAsync(pokemon);
            return pokemon;
        }

        public async Task AddRangeAsync(IEnumerable<Pokemon> pokemon)
        {
            await _context.Pokemon.AddRangeAsync(pokemon);
        }

        public async Task<bool> ExistsAsync(int pokedexNumber)
        {
            return await _context.Pokemon
                .AnyAsync(p => p.PokedexNumber == pokedexNumber);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
