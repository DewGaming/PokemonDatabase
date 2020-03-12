using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Update Pokemon Game Availability page.
    public class PokemonGameViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of pokemon game availability.
        /// </summary>
        public List<PokemonGameDetail> PokemonGameDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
