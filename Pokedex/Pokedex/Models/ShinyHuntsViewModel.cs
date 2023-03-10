using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the shiny hunts view model.
    /// </summary>
    public class ShinyHuntsViewModel
    {
        /// <summary>
        /// Gets or sets a list of all shiny hunts.
        /// </summary>
        public List<ShinyHunt> AllShinyHunts { get; set; }

        /// <summary>
        /// Gets or sets a list of all editted games.
        /// </summary>
        public List<Game> EdittedGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all uneditted games.
        /// </summary>
        public List<Game> UnedittedGames { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
