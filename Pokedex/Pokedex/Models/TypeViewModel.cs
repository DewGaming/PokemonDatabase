using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the typing view model.
    /// </summary>
    public class TypeViewModel
    {
        /// <summary>
        /// Gets or sets a list of all types.
        /// </summary>
        public List<Type> AllTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon type details.
        /// </summary>
        public List<PokemonTypeDetail> AllPokemon { get; set; }
    }
}
