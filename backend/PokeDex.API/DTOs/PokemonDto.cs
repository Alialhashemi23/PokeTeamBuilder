namespace PokeDex.API.DTOs
{
    /// <summary>
    /// Data Transfer Object for Pokemon API responses
    /// Separates API contract from database entities
    /// </summary>
    public class PokemonDto
    {
        public int Id { get; set; }
        public int PokedexNumber { get; set; }
        public string Name { get; set; } = string.Empty;

        // Stats
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }

        // Types (nested objects for frontend)
        public PokemonTypeDto PrimaryType { get; set; } = null!;
        public PokemonTypeDto? SecondaryType { get; set; }
    }
}
