using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the shiny hunting page view model.
    /// </summary>
    public class ShinyHuntingViewModel
    {
        /// <summary>
        /// Gets or sets a list of shiny hunts in progress.
        /// </summary>
        public List<ShinyHunt> InProgressHunts { get; set; }

        /// <summary>
        /// Gets or sets a list of shiny hunts that are completed.
        /// </summary>
        public List<ShinyHunt> CompletedHunts { get; set; }

        /// <summary>
        /// Gets or sets a list of shiny hunts that were failed.
        /// </summary>
        public List<ShinyHunt> FailedHunts { get; set; }

        /// <summary>
        /// Gets or sets the pokemon image location.
        /// </summary>
        public string PokemonImageLocation { get; set; }
    }
}
