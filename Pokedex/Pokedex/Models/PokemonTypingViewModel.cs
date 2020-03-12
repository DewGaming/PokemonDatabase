using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represet the pokemon typing view model.
    /// </summary>
    public class PokemonTypingViewModel : PokemonTypeDetail
    {
        /// <summary>
        /// Gets or sets a list of all types.
        /// </summary>
        public List<Type> AllTypes { get; set; }
    }
}
