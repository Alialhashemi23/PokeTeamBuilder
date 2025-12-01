using PokeDex.Core.Models;

namespace PokeDex.Data.Repositories
{
    public interface IPokemonRepository
    {
        Task<Pokemon?> GetByPokedexNumberAsync(int pokedexNumber);
        Task<List<Pokemon>> GetAllAsync();
        Task<Pokemon> AddAsync(Pokemon pokemon);
        Task AddRangeAsync(IEnumerable<Pokemon> pokemon);
        Task<bool> ExistsAsync(int pokedexNumber);
        Task<int> SaveChangesAsync();
    }
}
