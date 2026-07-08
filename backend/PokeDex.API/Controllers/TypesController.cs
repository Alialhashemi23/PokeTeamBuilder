using Microsoft.AspNetCore.Mvc;
using PokeDex.Core.Services;

namespace PokeDex.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypesController : ControllerBase
    {
        /// <summary>
        /// The full 18x18 type effectiveness chart:
        /// attacking type -> defending type -> multiplier
        /// </summary>
        [HttpGet("effectiveness")]
        public ActionResult<Dictionary<string, Dictionary<string, double>>> GetEffectivenessChart()
        {
            var chart = TypeChart.Types.ToDictionary(
                attacking => attacking,
                attacking => TypeChart.Types.ToDictionary(
                    defending => defending,
                    defending => TypeChart.GetMultiplier(attacking, defending)));

            return Ok(chart);
        }
    }
}
