using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add and Update Evolution pages.
    /// </summary>
    public class EditEvolutionViewModel : Evolution
    {
        /// <summary>
        /// Gets or sets a list of all evolutions.
        /// </summary>
        public List<Evolution> AllEvolutions { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
