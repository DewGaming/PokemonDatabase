using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location time view model.
    /// </summary>
    public class PokemonLocationTimeDetailViewModel : PokemonLocationTimeDetail
    {
        /// <summary>
        /// Gets or sets a list of all times.
        /// </summary>
        public List<Time> AllTimes { get; set; }

        /// <summary>
        /// Gets or sets the location id.
        /// </summary>
        public int LocationId { get; set; }
    }
}
