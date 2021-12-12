using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the location availability view model.
    /// </summary>
    public class LocationAvailabilityViewModel
    {
        /// <summary>
        /// Gets or sets a list of all locations.
        /// </summary>
        public List<Location> AllLocations { get; set; }

        /// <summary>
        /// Gets or sets the id of the game.
        /// </summary>
        public int GameId { get; set; }
    }
}
