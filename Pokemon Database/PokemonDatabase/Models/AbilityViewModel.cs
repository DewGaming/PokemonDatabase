using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class AbilityViewModel
    {
        public List<Ability> AllAbilities { get; set; }
        public List<PokemonAbilityDetail> AllPokemon { get; set; }
    }
}