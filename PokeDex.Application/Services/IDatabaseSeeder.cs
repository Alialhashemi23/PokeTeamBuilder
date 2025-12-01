namespace PokeDex.Application.Services
{
    public interface IDatabaseSeeder
    {
        Task<SeedResult> SeedDatabaseAsync(int pokemonCount = 151);
    }

    public class SeedResult
    {
        public int PokemonFetched { get; set; }
        public int PokemonAdded { get; set; }
        public int PokemonSkipped { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
