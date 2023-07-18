using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon difference view model.
    /// </summary>
    public class PokemonDifferenceSharedViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonViewModel> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets the games selected.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
