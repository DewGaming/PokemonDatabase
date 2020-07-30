using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the type chart page view model.
    /// </summary>
    public class PokemonTypeChartViewModel
    {
        /// <summary>
        /// Gets or sets a list of type charts.
        /// </summary>
        public List<TypeChart> TypeChart { get; set; }

        /// <summary>
        /// Gets or sets the generation.
        /// </summary>
        public Generation Generation { get; set; }
    }
}
