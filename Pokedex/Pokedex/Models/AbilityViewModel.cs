using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ability view model.
    /// </summary>
    public class AbilityViewModel
    {
        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public List<Ability> AllAbilities { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon ability details.
        /// </summary>
        public List<PokemonAbilityDetail> AllPokemon { get; set; }
    }
}
