using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin egg group page view model.
    /// </summary>
    public class EggGroupViewModel
    {
        /// <summary>
        /// Gets or sets a list of all egg groups.
        /// </summary>
        public List<EggGroup> AllEggGroups { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon egg group details.
        /// </summary>
        public List<PokemonEggGroupDetail> AllPokemon { get; set; }
    }
}
