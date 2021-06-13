using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the admin pokemon location view model.
    /// </summary>
    public class PokemonLocationDetailAdminViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonLocationDetail> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets the location id.
        /// </summary>
        public int LocationId { get; set; }
    }
}
