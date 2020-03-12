using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Gender Ratio page.
    /// </summary>
    public class GenderRatiosViewModel
    {
        /// <summary>
        /// Gets or sets a list of all gender ratios.
        /// </summary>
        public List<GenderRatio> AllGenderRatios { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
