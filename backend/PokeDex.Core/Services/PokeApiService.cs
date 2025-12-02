using Newtonsoft.Json;
using PokeDex.Core.Models.DTOs;

namespace PokeDex.Core.Services
{
    public class PokeApiService : IPokeApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://pokeapi.co/api/v2/";
        private const int MaxConcurrentRequests = 10;

        public PokeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<List<PokemonApiDto>> FetchPokemonAsync(int count = 30)
        {
            using var semaphore = new SemaphoreSlim(MaxConcurrentRequests);

            var tasks = Enumerable.Range(1, count)
                .Select(async i =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await FetchSinglePokemonAsync(i);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            var results = await Task.WhenAll(tasks);

            return results.Where(p => p != null).ToList()!;
        }

        private async Task<PokemonApiDto?> FetchSinglePokemonAsync(int pokemonId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pokemon/{pokemonId}/");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var pokeApiData = JsonConvert.DeserializeObject<PokeApiPokemon>(json);

                if (pokeApiData == null)
                {
                    return null;
                }

                return new PokemonApiDto
                {
                    PokedexNumber = pokeApiData.id,
                    Name = pokeApiData.name,
                    HP = pokeApiData.stats.First(s => s.stat.name == "hp").base_stat,
                    Attack = pokeApiData.stats.First(s => s.stat.name == "attack").base_stat,
                    Defense = pokeApiData.stats.First(s => s.stat.name == "defense").base_stat,
                    SpecialAttack = pokeApiData.stats.First(s => s.stat.name == "special-attack").base_stat,
                    SpecialDefense = pokeApiData.stats.First(s => s.stat.name == "special-defense").base_stat,
                    Speed = pokeApiData.stats.First(s => s.stat.name == "speed").base_stat,
                    PrimaryType = pokeApiData.types.FirstOrDefault(t => t.slot == 1)?.type.name ?? string.Empty,
                    SecondaryType = pokeApiData.types.FirstOrDefault(t => t.slot == 2)?.type.name
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Pokémon ID {pokemonId}: {ex.Message}");
                return null;
            }
        }
        private class PokeApiPokemon
        {
            public int id { get; set; }
            public string name { get; set; } = string.Empty;
            public List<PokemonStat> stats { get; set; } = new();
            public List<PokemonTypeSlot> types { get; set; } = new();
        }

        private class PokemonStat
        {
            public int base_stat { get; set; }
            public StatInfo stat { get; set; } = new();
        }

        private class StatInfo
        {
            public string name { get; set; } = string.Empty;
        }

        private class PokemonTypeSlot
        {
            public int slot { get; set; }
            public TypeInfo type { get; set; } = new();
        }

        private class TypeInfo
        {
            public string name { get; set; } = string.Empty;
        }
    }
}