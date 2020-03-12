using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Legendary Type page.
    /// </summary>
    public class LegendaryTypeViewModel
    {
        /// <summary>
        /// Gets or sets a list of all legendary types.
        /// </summary>
        public List<LegendaryType> AllLegendaryTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon legendary types details.
        /// </summary>
        public List<PokemonLegendaryDetail> AllPokemon { get; set; }
    }
}
