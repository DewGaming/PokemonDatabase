using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add and Update Pokemon Egg Groups page.
    /// </summary>
    public class PokemonEggGroupsViewModel : PokemonEggGroupDetail
    {
        /// <summary>
        /// Gets or sets a list of all egg groups.
        /// </summary>
        public List<EggGroup> AllEggGroups { get; set; }
    }
}
