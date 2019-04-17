using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AbilityViewModel
    {
        public List<Ability> AllAbilities { get; set; }
        public List<PokemonAbilityDetail> AllPokemon { get; set; }
    }
}