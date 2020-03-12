using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Experience Growth page.
    /// </summary>
    public class ExperienceGrowthViewModel
    {
        /// <summary>
        /// Gets or sets a list of all experience growths.
        /// </summary>
        public List<ExperienceGrowth> AllExperienceGrowths { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
