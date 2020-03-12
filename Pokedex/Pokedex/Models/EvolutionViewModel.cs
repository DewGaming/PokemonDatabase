using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add and Update Evolution pages.
    /// </summary>
    public class EvolutionViewModel : Evolution
    {
        /// <summary>
        /// Gets or sets a list of all evolution methods.
        /// </summary>
        public List<EvolutionMethod> AllEvolutionMethods { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
