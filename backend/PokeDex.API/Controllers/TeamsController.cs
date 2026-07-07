using Microsoft.AspNetCore.Mvc;
using PokeDex.API.DTOs;
using PokeDex.Application.Services;

namespace PokeDex.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        /// <summary>
        /// Get all teams with their members
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllTeams()
        {
            try
            {
                var teams = await _teamService.GetAllTeamsAsync();
                return Ok(teams.Select(t => t.ToDto()).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teams");
                return StatusCode(500, "An error occurred while retrieving teams");
            }
        }

        /// <summary>
        /// Get a single team by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeamById(int id)
        {
            try
            {
                var team = await _teamService.GetTeamByIdAsync(id);
                if (team == null)
                {
                    return NotFound($"Team with ID {id} not found");
                }

                return Ok(team.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team {TeamId}", id);
                return StatusCode(500, "An error occurred while retrieving the team");
            }
        }

        /// <summary>
        /// Create a new team
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamRequest request)
        {
            try
            {
                var result = await _teamService.CreateTeamAsync(request.Name);
                if (!result.Success)
                {
                    return ErrorResponse(result);
                }

                var dto = result.Team!.ToDto();
                return CreatedAtAction(nameof(GetTeamById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team");
                return StatusCode(500, "An error occurred while creating the team");
            }
        }

        /// <summary>
        /// Rename a team
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TeamDto>> RenameTeam(int id, [FromBody] RenameTeamRequest request)
        {
            try
            {
                var result = await _teamService.RenameTeamAsync(id, request.Name);
                if (!result.Success)
                {
                    return ErrorResponse(result);
                }

                return Ok(result.Team!.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renaming team {TeamId}", id);
                return StatusCode(500, "An error occurred while renaming the team");
            }
        }

        /// <summary>
        /// Delete a team and all of its member slots
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            try
            {
                var result = await _teamService.DeleteTeamAsync(id);
                if (!result.Success)
                {
                    return ErrorResponse(result);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team {TeamId}", id);
                return StatusCode(500, "An error occurred while deleting the team");
            }
        }

        /// <summary>
        /// Add a Pokemon to a team (max 6, no duplicates)
        /// </summary>
        [HttpPost("{id}/pokemon")]
        public async Task<ActionResult<TeamDto>> AddPokemon(int id, [FromBody] AddTeamMemberRequest request)
        {
            try
            {
                var result = await _teamService.AddPokemonAsync(id, request.PokemonId);
                if (!result.Success)
                {
                    return ErrorResponse(result);
                }

                return Ok(result.Team!.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Pokemon {PokemonId} to team {TeamId}", request.PokemonId, id);
                return StatusCode(500, "An error occurred while adding the Pokemon to the team");
            }
        }

        /// <summary>
        /// Remove a member slot from a team
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <param name="teamPokemonId">The member slot ID (TeamMemberDto.TeamPokemonId), not the Pokemon ID</param>
        [HttpDelete("{id}/pokemon/{teamPokemonId}")]
        public async Task<ActionResult<TeamDto>> RemovePokemon(int id, int teamPokemonId)
        {
            try
            {
                var result = await _teamService.RemovePokemonAsync(id, teamPokemonId);
                if (!result.Success)
                {
                    return ErrorResponse(result);
                }

                return Ok(result.Team!.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing member {TeamPokemonId} from team {TeamId}", teamPokemonId, id);
                return StatusCode(500, "An error occurred while removing the Pokemon from the team");
            }
        }

        private ActionResult ErrorResponse(TeamResult result)
        {
            return result.Error switch
            {
                TeamError.TeamNotFound or TeamError.PokemonNotFound or TeamError.MemberNotFound
                    => NotFound(result.Message),
                TeamError.TeamFull or TeamError.DuplicatePokemon
                    => Conflict(result.Message),
                _ => BadRequest(result.Message)
            };
        }
    }
}
