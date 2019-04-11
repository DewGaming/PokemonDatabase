using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class PokemonEggGroupsViewModel : PokemonEggGroupDetail
    {
        public List<EggGroup> AllEggGroups { get; set; }
    }
}