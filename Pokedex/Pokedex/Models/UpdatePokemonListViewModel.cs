using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent  the update pokemon list ajax view model.
    /// </summary>
    public class UpdatePokemonListViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon list that is used for the update.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets the game for the pokemon list.
        public Game Game { get; set; }
    }
}
