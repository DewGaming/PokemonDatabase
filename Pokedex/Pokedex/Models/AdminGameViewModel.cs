using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the admin game view model.
    /// </summary>
    public class AdminGameViewModel
    {
        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all game region details.
        /// </summary>
        public List<GameRegionDetail> AllGameRegionDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
