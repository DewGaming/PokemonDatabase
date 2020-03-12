using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the beginning shiny hunt page view model.
    /// </summary>
    public class BeginShinyHuntViewModel : ShinyHunt
    {
        /// <summary>
        /// Gets or sets a list of all shiny hunting technique.
        /// </summary>
        public List<ShinyHuntingTechnique> AllShinyHuntingTechniques { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
