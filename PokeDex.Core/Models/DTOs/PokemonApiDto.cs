namespace PokeDex.Core.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object representing Pokemon data from the external PokeAPI.
    /// This keeps the API service decoupled from database concerns (type IDs, etc.)
    /// </summary>
    public class PokemonApiDto
    {
        public int PokedexNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public string PrimaryType { get; set; } = string.Empty;
        public string? SecondaryType { get; set; }
    }
}
