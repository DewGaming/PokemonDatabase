using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the region view model.
    /// </summary>
    public class LocationAdminViewModel
    {
        /// <summary>
        /// Gets or sets a list of all regions.
        /// </summary>
        public List<Location> AllLocations { get; set; }

        /// <summary>
        /// Gets or sets a list of all regions.
        /// </summary>
        public List<PokemonLocationDetail> AllPokemonLocationDetails { get; set; }
    }
}
