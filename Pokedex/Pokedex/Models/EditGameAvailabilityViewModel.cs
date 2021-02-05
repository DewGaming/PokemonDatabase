using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the edit game availability view model.
    /// </summary>
    public class EditGameAvailabilityViewModel
    {
        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> Games { get; set; }

        /// <summary>
        /// Gets or sets the game who's availability is being edited.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonGameDetail> GameAvailability { get; set; }
    }
}
