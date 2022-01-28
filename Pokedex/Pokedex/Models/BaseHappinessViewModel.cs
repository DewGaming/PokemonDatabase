using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the base happiness view model.
    /// </summary>
    public class BaseHappinessViewModel
    {
        /// <summary>
        /// Gets or sets a list of all base happiness.
        /// </summary>
        public List<BaseHappiness> AllBaseHappinesses { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonBaseHappinessDetail> AllPokemon { get; set; }
    }
}
