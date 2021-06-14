using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location game view model.
    /// </summary>
    public class PokemonLocationGameDetailViewModel
    {
        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon locaiton detail.
        /// </summary>
        public PokemonLocationDetail PokemonLocationDetail { get; set; }

        /// <summary>
        /// Gets or sets the selected game id.
        /// </summary>
        public List<int> GameIds { get; set; }
    }
}
