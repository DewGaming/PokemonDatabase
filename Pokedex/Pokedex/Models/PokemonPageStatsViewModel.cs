using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Update Pokemon ajax call.
    /// </summary>
    public class PokemonPageStatsViewModel
    {
        /// <summary>
        /// Gets or sets a list of pokemon.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets a list of page stats for pokemon.
        /// </summary>
        public List<PokemonPageStat> PageStatList { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the latest generation.
        /// </summary>
        public Generation Generation { get; set; }
    }
}
