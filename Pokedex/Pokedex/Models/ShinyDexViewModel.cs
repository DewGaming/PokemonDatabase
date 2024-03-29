using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the shiny dex view model.
    /// </summary>
    public class ShinyDexViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonShinyHuntDetails> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all shiny hunts.
        /// </summary>
        public List<ShinyHunt> AllShinyHunts { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this page is shared.
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        public User CurrentUser { get; set; }
    }
}
