using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class PokemonViewModel
    {
        public Pokemon pokemon { get; set; }
        public BaseStat baseStats { get; set; }
        public EVYield evYields { get; set; }
        public List<Type> types { get; set; }
        public List<Ability> abilities { get; set; }
        public List<EggGroup> eggGroups { get; set; }
        public Evolution preEvolution { get; set; }
        public List<Evolution> evolutions { get; set; }
        public List<PokemonFormDetail> forms { get; set; }
        public PokemonFormDetail originalForm { get; set; }
        public TypeEffectivenessViewModel effectiveness { get; set; }
    }
}