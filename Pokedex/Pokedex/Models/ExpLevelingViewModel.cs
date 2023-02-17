using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the exp leveling view model.
    /// </summary>
    public class ExpLevelingViewModel
    {
        /// <summary>
        /// Gets or sets the list of pokemon.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
