using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the type chart page view model.
    /// </summary>
    public class TypeChartViewModel
    {
        /// <summary>
        /// Gets or sets a list of type charts.
        /// </summary>
        public List<TypeChart> TypeChart { get; set; }

        /// <summary>
        /// Gets or sets a list of types.
        /// </summary>
        public List<Type> Types { get; set; }
    }
}
