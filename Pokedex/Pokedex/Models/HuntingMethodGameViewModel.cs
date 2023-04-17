using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Update Hunting Method Game Availability page.
    public class HuntingMethodGameViewModel
    {
        /// <summary>
        /// Gets or sets the hunting method.
        /// </summary>
        public HuntingMethod HuntingMethod { get; set; }

        /// <summary>
        /// Gets or sets a list of hunting method game availability.
        /// </summary>
        public List<HuntingMethodGameDetail> HuntingMethodGameDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
