using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeDex.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        // Identity user ID of the owner. Kept as a plain string (no navigation)
        // so the Core project stays free of Identity dependencies.
        public string? OwnerId { get; set; }
        public ICollection<TeamPokemon> TeamPokemon { get; set; } = new List<TeamPokemon>();
    }
}
