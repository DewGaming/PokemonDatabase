using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonAbilitiesViewModel : PokemonAbilityDetail
    {
        public List<Ability> AllAbilities { get; set; }
    }
}