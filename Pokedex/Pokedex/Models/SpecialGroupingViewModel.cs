using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the special grouping's view model.
    /// </summary>
    public class SpecialGroupingViewModel
    {
        /// <summary>
        /// Gets or sets a list of all special groupings.
        /// </summary>
        public List<SpecialGrouping> AllSpecialGroupings { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
