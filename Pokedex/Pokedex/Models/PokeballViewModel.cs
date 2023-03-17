using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokeball view model.
    /// </summary>
    public class PokeballViewModel : Pokeball
    {
        /// <summary>
        /// Gets or sets a list of all generations.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }
    }
}
