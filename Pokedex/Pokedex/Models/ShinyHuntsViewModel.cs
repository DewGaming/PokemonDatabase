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
        /// Gets or sets the user of the shiny hunts.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
