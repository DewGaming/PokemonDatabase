using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the edit regional dex entries view model.
    /// </summary>
    public class EditRegionalDexEntriesViewModel
    {
        /// <summary>
        /// Gets or sets the regional dex.
        /// </summary>
        public RegionalDex RegionalDex { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets a list of overall regional dex entries.
        /// </summary>
        public List<RegionalDexEntry> RegionalDexEntries { get; set; }
    }
}
