using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonViewModel
    {
        public Pokemon pokemon { get; set; }
        public BaseStat baseStats { get; set; }
        public EVYield evYields { get; set; }
        public Type PrimaryType { get; set; }
        public Type SecondaryType { get; set; }
        public Ability PrimaryAbility { get; set; }
        public Ability SecondaryAbility { get; set; }
        public Ability HiddenAbility { get; set; }
        public EggGroup PrimaryEggGroup { get; set; }
        public EggGroup SecondaryEggGroup { get; set; }
        public Evolution preEvolution { get; set; }
        public List<Evolution> evolutions { get; set; }
        public TypeEffectivenessViewModel effectiveness { get; set; }
    }
}