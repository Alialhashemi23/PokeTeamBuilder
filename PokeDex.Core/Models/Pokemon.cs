using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeDex.Core.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public int PokedexNumber { get; set; }
        public string Name { get; set; } = string.Empty;

        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }

        public int PrimaryTypeId { get; set; }
        public int? SecondaryTypeId { get; set; }

        public PokemonType PrimaryType { get; set; }
        public PokemonType? SecondaryType { get; set; }
    }
}
