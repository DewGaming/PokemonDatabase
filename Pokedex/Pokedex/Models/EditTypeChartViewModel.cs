using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Edit Type Chart page view model.
    /// </summary>
    public class EditTypeChartViewModel
    {
        /// <summary>
        /// Gets or sets a list of type charts.
        /// </summary>
        public List<TypeChart> TypeChart { get; set; }

        /// <summary>
        /// Gets or sets a list of types.
        /// </summary>
        public List<Type> Types { get; set; }

        /// <summary>
        /// Gets or sets the type id.
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Gets or sets the generation id.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
