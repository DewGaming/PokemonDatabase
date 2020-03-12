using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the classification's view model.
    /// </summary>
    public class ClassificationViewModel
    {
        /// <summary>
        /// Gets or sets a list of all classifications.
        /// </summary>
        public List<Classification> AllClassifications { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
