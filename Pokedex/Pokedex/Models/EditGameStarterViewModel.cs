using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the edit game starters view model.
    /// </summary>
    public class EditGameStarterViewModel
    {
        /// <summary>
        /// Gets or sets the game who's starters is being edited.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets a list of all starters.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets a list of all starters.
        /// </summary>
        public List<GameStarterDetail> GameStarters { get; set; }
    }
}
