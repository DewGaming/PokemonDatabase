using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the admin game location view model.
    /// </summary>
    public class AdminGameLocationViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon's Id.
        /// </summary>
        public int PokemonId { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all locations.
        /// </summary>
        public List<PokemonLocation> AllLocations { get; set; }
    }
}
