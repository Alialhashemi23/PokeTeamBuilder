using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeDex.Core.Models;
using PokeDex.Core.Models.DTOs;
using PokeDex.Core.Services;
using PokeDex.Data;
using PokeDex.Data.Repositories;

namespace PokeDex.Application.Services
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly IPokeApiService _pokeApiService;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly PokedexDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            IPokeApiService pokeApiService,
            IPokemonRepository pokemonRepository,
            PokedexDbContext context,
            ILogger<DatabaseSeeder> logger)
        {
            _pokeApiService = pokeApiService;
            _pokemonRepository = pokemonRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<SeedResult> SeedDatabaseAsync(int pokemonCount = 151)
        {
            var result = new SeedResult();

            try
            {
                _logger.LogInformation("Starting database seeding...");

                _logger.LogInformation("Fetching {Count} Pokemon from PokeAPI...", pokemonCount);
                var fetchedPokemonDtos = await _pokeApiService.FetchPokemonAsync(pokemonCount);
                result.PokemonFetched = fetchedPokemonDtos.Count;
                _logger.LogInformation("Fetched {Count} Pokemon from API", fetchedPokemonDtos.Count);

                var typeMap = await _context.PokemonTypes
                    .ToDictionaryAsync(t => t.Name.ToLower(), t => t.Id);

                var newPokemon = new List<Pokemon>();
                foreach (var dto in fetchedPokemonDtos)
                {
                    var existing = await _pokemonRepository.GetByPokedexNumberAsync(dto.PokedexNumber);
                    if (existing == null)
                    {
                        if (!typeMap.TryGetValue(dto.PrimaryType.ToLower(), out var primaryTypeId))
                        {
                            _logger.LogWarning("Primary type {PrimaryType} not found for Pokemon {PokemonName}. Skipping.", dto.PrimaryType, dto.Name);
                            result.PokemonSkipped++;
                            continue;
                        }

                        int? secondaryTypeId = null;
                        if (!string.IsNullOrEmpty(dto.SecondaryType))
                        {
                            if (typeMap.TryGetValue(dto.SecondaryType.ToLower(), out var foundSecondaryTypeId))
                            {
                                secondaryTypeId = foundSecondaryTypeId;
                            }
                            else
                            {
                                _logger.LogWarning("Secondary type {SecondaryType} not found for Pokemon {PokemonName}. Setting to null.", dto.SecondaryType, dto.Name);
                            }
                        }
                        
                        var pokemon = new Pokemon
                        {
                            PokedexNumber = dto.PokedexNumber,
                            Name = dto.Name,
                            HP = dto.HP,
                            Attack = dto.Attack,
                            Defense = dto.Defense,
                            SpecialAttack = dto.SpecialAttack,
                            SpecialDefense = dto.SpecialDefense,
                            Speed = dto.Speed,
                            SpriteUrl = dto.SpriteUrl,
                            PrimaryTypeId = primaryTypeId,
                            SecondaryTypeId = secondaryTypeId
                        };
                        newPokemon.Add(pokemon);
                    }
                    else if (!string.IsNullOrEmpty(dto.SpriteUrl) && existing.SpriteUrl != dto.SpriteUrl)
                    {
                        // Backfill sprites for Pokemon seeded before sprites were tracked
                        existing.SpriteUrl = dto.SpriteUrl;
                        result.PokemonUpdated++;
                    }
                    else
                    {
                        result.PokemonSkipped++;
                    }
                }

                if (newPokemon.Any())
                {
                    await _pokemonRepository.AddRangeAsync(newPokemon);
                    result.PokemonAdded = newPokemon.Count;
                }

                if (newPokemon.Any() || result.PokemonUpdated > 0)
                {
                    await _pokemonRepository.SaveChangesAsync();
                    _logger.LogInformation(
                        "Added {Added} new Pokemon, updated {Updated}", result.PokemonAdded, result.PokemonUpdated);
                }

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database seeding");
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
