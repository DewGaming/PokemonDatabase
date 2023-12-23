using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the edit pokeball game availability view model.
    /// </summary>
    public class EditPokeballGameViewModel
    {
        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokeballs.
        /// </summary>
        public List<Pokeball> AllPokeballs { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokeball game details.
        /// </summary>
        public List<PokeballGameDetail> PokeballGameDetails { get; set; }
    }
}
