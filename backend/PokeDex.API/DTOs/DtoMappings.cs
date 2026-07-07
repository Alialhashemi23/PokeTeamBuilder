using PokeDex.Core.Models;

namespace PokeDex.API.DTOs
{
    /// <summary>
    /// Entity-to-DTO mapping helpers shared by controllers
    /// </summary>
    public static class DtoMappings
    {
        public static PokemonDto ToDto(this Pokemon pokemon)
        {
            return new PokemonDto
            {
                Id = pokemon.Id,
                PokedexNumber = pokemon.PokedexNumber,
                Name = pokemon.Name,
                HP = pokemon.HP,
                Attack = pokemon.Attack,
                Defense = pokemon.Defense,
                SpecialAttack = pokemon.SpecialAttack,
                SpecialDefense = pokemon.SpecialDefense,
                Speed = pokemon.Speed,
                SpriteUrl = pokemon.SpriteUrl,
                PrimaryType = pokemon.PrimaryType.ToDto(),
                SecondaryType = pokemon.SecondaryType?.ToDto()
            };
        }

        public static PokemonTypeDto ToDto(this PokemonType type)
        {
            return new PokemonTypeDto
            {
                Id = type.Id,
                Name = type.Name
            };
        }

        public static TeamDto ToDto(this Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                CreatedDate = team.CreatedDate,
                Members = team.TeamPokemon
                    .OrderBy(tp => tp.Id)
                    .Select(tp => new TeamMemberDto
                    {
                        TeamPokemonId = tp.Id,
                        Pokemon = tp.Pokemon.ToDto()
                    })
                    .ToList()
            };
        }
    }
}
