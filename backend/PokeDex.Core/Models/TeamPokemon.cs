using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeDex.Core.Models
{
    public class TeamPokemon
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int PokemonId { get; set; }
        public Team Team { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}
