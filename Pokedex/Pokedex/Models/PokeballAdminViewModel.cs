using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ability view model.
    /// </summary>
    public class PokeballAdminViewModel : Pokeball
    {
        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }
    }
}
