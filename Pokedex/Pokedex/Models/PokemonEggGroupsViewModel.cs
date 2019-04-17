using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonEggGroupsViewModel : PokemonEggGroupDetail
    {
        public List<EggGroup> AllEggGroups { get; set; }
    }
}