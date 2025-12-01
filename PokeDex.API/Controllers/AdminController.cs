using Microsoft.AspNetCore.Mvc;
using PokeDex.Application.Services;

namespace PokeDex.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IDatabaseSeeder _databaseSeeder;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IDatabaseSeeder databaseSeeder, ILogger<AdminController> logger)
        {
            _databaseSeeder = databaseSeeder;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database with Pokemon types and Pokemon data from PokeAPI
        /// </summary>
        /// <param name="count">Number of Pokemon to fetch (default: 151 for Gen 1)</param>
        /// <returns>Seeding results including counts and any errors</returns>
        [HttpPost("seed")]
        public async Task<ActionResult<SeedResult>> SeedDatabase([FromQuery] int count = 151)
        {
            if (count <= 0 || count > 1000)
            {
                return BadRequest("Count must be between 1 and 1000");
            }

            _logger.LogInformation("Seed endpoint triggered for {Count} Pokemon", count);

            var result = await _databaseSeeder.SeedDatabaseAsync(count);

            if (result.Errors.Any())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
    }
}
