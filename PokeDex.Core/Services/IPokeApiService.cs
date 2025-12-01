using PokeDex.Core.Models.DTOs;

namespace PokeDex.Core.Services
{
    public interface IPokeApiService
    {
        Task<List<PokemonApiDto>> FetchPokemonAsync(int count = 30);
    }
}
