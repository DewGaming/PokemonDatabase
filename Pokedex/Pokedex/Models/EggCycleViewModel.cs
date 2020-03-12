using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Egg Cycle page view model.
    /// </summary>
    public class EggCycleViewModel
    {
        /// <summary>
        /// Gets or sets a list of all egg cycles.
        /// </summary>
        public List<EggCycle> AllEggCycles { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
