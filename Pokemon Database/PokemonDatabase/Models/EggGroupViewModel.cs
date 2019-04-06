using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class EggGroupViewModel
    {
        public List<EggGroup> AllEggGroups { get; set; }
        public List<PokemonEggGroupDetail> AllPokemon { get; set; }
    }
}