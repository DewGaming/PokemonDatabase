using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent a pokemon's admin dropdown menu.
    /// </summary>
    public class GameAvailabilityViewModel
    {
        /// <summary>
        /// Gets or sets the editted games list.
        /// </summary>
        public List<Game> EdittedGames { get; set; }

        /// <summary>
        /// Gets or sets the uneditted games list.
        /// </summary>
        public List<Game> UnedittedGames { get; set; }

        /// <summary>
        /// Gets or sets the Generation.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
