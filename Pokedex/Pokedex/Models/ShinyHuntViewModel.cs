using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add Shiny Hunting Technique page view model.
    /// </summary>
    public class ShinyHuntViewModel
    {
        /// <summary>
        /// Gets or sets a list of all shiny hunting techniques.
        /// </summary>
        public List<ShinyHuntingTechnique> AllShinyHuntingTechniques { get; set; }

        /// <summary>
        /// Gets or sets a list of shiny hunts.
        /// </summary>
        public List<ShinyHunt> AllShinyHunters { get; set; }
    }
}
