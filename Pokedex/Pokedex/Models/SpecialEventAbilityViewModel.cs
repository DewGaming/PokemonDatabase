using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the special event ability view model.
    /// </summary>
    public class SpecialEventAbilityViewModel
    {
        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public List<Ability> AllAbilities { get; set; }

        /// <summary>
        /// Gets or sets the specified ability's id.
        /// </summary>
        public int AbilityId { get; set; }

        /// <summary>
        /// Gets or sets the specified pokemon's id.
        /// </summary>
        public int PokemonId { get; set; }
    }
}
