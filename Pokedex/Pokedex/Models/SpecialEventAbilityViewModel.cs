using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class SpecialEventAbilityViewModel
    {
        public List<Ability> AllAbilities { get; set; }

        public int AbilityId { get; set; }

        public string PokemonId { get; set; }
    }
}