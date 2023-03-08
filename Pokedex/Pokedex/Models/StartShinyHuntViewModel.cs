using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the create pokemon team page view model.
    /// </summary>
    public class StartShinyHuntViewModel : ShinyHunt
    {
        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets the application's config.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
