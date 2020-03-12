using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Evolution Methods page.
    /// </summary>
    public class EvolutionMethodViewModel
    {
        /// <summary>
        /// Gets or sets a list of all evolution methods.
        /// </summary>
        public List<EvolutionMethod> AllEvolutionMethods { get; set; }

        /// <summary>
        /// Gets or sets a list of all evolutions.
        /// </summary>
        public List<Evolution> AllEvolutions { get; set; }
    }
}
