using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the capture calculator view model.
    /// </summary>
    public class CaptureCalculatorViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokeballs.
        /// </summary>
        public List<Pokeball> AllPokeballs { get; set; }
    }
}
