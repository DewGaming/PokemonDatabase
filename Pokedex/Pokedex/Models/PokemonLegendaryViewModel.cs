using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the view model for the Add Legendary View Model page.
    /// </summary>
    public class PokemonLegendaryViewModel : PokemonLegendaryDetail
    {
        /// <summary>
        /// Gets or sets the list of all legendary types.
        /// </summary>
        public List<LegendaryType> AllLegendaryTypes { get; set; }
    }
}
