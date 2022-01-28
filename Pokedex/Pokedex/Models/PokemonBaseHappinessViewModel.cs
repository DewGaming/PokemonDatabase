using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the base happiness view model.
    /// </summary>
    public class PokemonBaseHappinessViewModel : PokemonBaseHappinessDetail
    {
        /// <summary>
        /// Gets or sets a list of all base happiness.
        /// </summary>
        public List<BaseHappiness> AllBaseHappinesses { get; set; }
    }
}
