using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokeball view model.
    /// </summary>
    public class PokeballViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokeballs.
        /// </summary>
        public List<Pokeball> AllPokeballs { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokeball catch modifiers.
        /// </summary>
        public List<PokeballCatchModifierDetail> AllCatchModifiers { get; set; }
    }
}
