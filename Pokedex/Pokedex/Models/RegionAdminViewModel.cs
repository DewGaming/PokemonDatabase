using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the region view model.
    /// </summary>
    public class RegionAdminViewModel : Region
    {
        /// <summary>
        /// Gets or sets a list of all regions.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }
    }
}
