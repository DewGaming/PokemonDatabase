using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location time view model.
    /// </summary>
    public class PokemonLocationTimeDetailViewModel
    {
        /// <summary>
        /// Gets or sets a list of all times.
        /// </summary>
        public List<Time> AllTimes { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon locaiton detail.
        /// </summary>
        public PokemonLocationDetail PokemonLocationDetail { get; set; }

        /// <summary>
        /// Gets or sets the selected time id.
        /// </summary>
        public List<int> TimeIds { get; set; }
    }
}
