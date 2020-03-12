using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add and Update Pokemon Ability page.
    /// </summary>
    public class PokemonAbilitiesViewModel : PokemonAbilityDetail
    {
        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public List<Ability> AllAbilities { get; set; }
    }
}
