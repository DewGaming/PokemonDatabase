using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the region view model.
    /// </summary>
    public class LocationViewModel : Location
    {
        /// <summary>
        /// Gets or sets a list of all regions.
        /// </summary>
        public List<Region> AllRegions { get; set; }
    }
}
