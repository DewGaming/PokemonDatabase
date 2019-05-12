using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EggGroupViewModel
    {
        public List<EggGroup> AllEggGroups { get; set; }

        public List<PokemonEggGroupDetail> AllPokemon { get; set; }
    }
}