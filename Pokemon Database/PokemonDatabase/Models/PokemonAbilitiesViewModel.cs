using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class PokemonAbilitiesViewModel : PokemonAbilityDetail
    {
        public List<Ability> AllAbilities { get; set; }
    }
}