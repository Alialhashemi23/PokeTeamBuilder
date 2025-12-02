using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokeDex.API.DTOs;
using PokeDex.Data.Repositories;

namespace PokeDex.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ILogger<PokemonController> _logger;

        public PokemonController(IPokemonRepository pokemonRepository, ILogger<PokemonController> logger)
        {
            _pokemonRepository = pokemonRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all Pokemon, ordered by Pokedex number
        /// </summary>
        /// <returns>List of Pokemon</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PokemonDto>>> GetAllPokemon()
        {
            try
            {
                var pokemon = await _pokemonRepository.GetAllAsync();

                var pokemonDtos = pokemon.Select(p => new PokemonDto
                {
                    Id = p.Id,
                    PokedexNumber = p.PokedexNumber,
                    Name = p.Name,
                    HP = p.HP,
                    Attack = p.Attack,
                    Defense = p.Defense,
                    SpecialAttack = p.SpecialAttack,
                    SpecialDefense = p.SpecialDefense,
                    Speed = p.Speed,
                    PrimaryType = new PokemonTypeDto
                    {
                        Id = p.PrimaryType.Id,
                        Name = p.PrimaryType.Name
                    },
                    SecondaryType = p.SecondaryType != null ? new PokemonTypeDto
                    {
                        Id = p.SecondaryType.Id,
                        Name = p.SecondaryType.Name
                    } : null
                }).ToList();

                return Ok(pokemonDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Pokemon list");
                return StatusCode(500, "An error occurred while retrieving Pokemon");
            }
        }

        /// <summary>
        /// Get a single Pokemon by ID
        /// </summary>
        /// <param name="id">Pokemon ID</param>
        /// <returns>Pokemon details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PokemonDto>> GetPokemonById(int id)
        {
            try
            {
                var pokemon = await _pokemonRepository.GetAllAsync();
                var foundPokemon = pokemon.FirstOrDefault(p => p.Id == id);

                if (foundPokemon == null)
                {
                    _logger.LogWarning("Pokemon with ID {PokemonId} not found", id);
                    return NotFound($"Pokemon with ID {id} not found");
                }

                var pokemonDto = new PokemonDto
                {
                    Id = foundPokemon.Id,
                    PokedexNumber = foundPokemon.PokedexNumber,
                    Name = foundPokemon.Name,
                    HP = foundPokemon.HP,
                    Attack = foundPokemon.Attack,
                    Defense = foundPokemon.Defense,
                    SpecialAttack = foundPokemon.SpecialAttack,
                    SpecialDefense = foundPokemon.SpecialDefense,
                    Speed = foundPokemon.Speed,
                    PrimaryType = new PokemonTypeDto
                    {
                        Id = foundPokemon.PrimaryType.Id,
                        Name = foundPokemon.PrimaryType.Name
                    },
                    SecondaryType = foundPokemon.SecondaryType != null ? new PokemonTypeDto
                    {
                        Id = foundPokemon.SecondaryType.Id,
                        Name = foundPokemon.SecondaryType.Name
                    } : null
                };

                return Ok(pokemonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Pokemon with ID {PokemonId}", id);
                return StatusCode(500, "An error occurred while retrieving the Pokemon");
            }
        }

        /// <summary>
        /// Get Pokemon by Pokedex number
        /// </summary>
        /// <param name="pokedexNumber">Pokedex number (e.g., 1 for Bulbasaur)</param>
        /// <returns>Pokemon details</returns>
        [HttpGet("pokedex/{pokedexNumber}")]
        public async Task<ActionResult<PokemonDto>> GetPokemonByPokedexNumber(int pokedexNumber)
        {
            try
            {
                var pokemon = await _pokemonRepository.GetByPokedexNumberAsync(pokedexNumber);

                if (pokemon == null)
                {
                    _logger.LogWarning("Pokemon with Pokedex number {PokedexNumber} not found", pokedexNumber);
                    return NotFound($"Pokemon #{pokedexNumber} not found");
                }

                var pokemonDto = new PokemonDto
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
                    PrimaryType = new PokemonTypeDto
                    {
                        Id = pokemon.PrimaryType.Id,
                        Name = pokemon.PrimaryType.Name
                    },
                    SecondaryType = pokemon.SecondaryType != null ? new PokemonTypeDto
                    {
                        Id = pokemon.SecondaryType.Id,
                        Name = pokemon.SecondaryType.Name
                    } : null
                };

                return Ok(pokemonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Pokemon with Pokedex number {PokedexNumber}", pokedexNumber);
                return StatusCode(500, "An error occurred while retrieving the Pokemon");
            }
        }
    }
}
