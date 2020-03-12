using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Pokemon page view model.
    /// </summary>
    public class AdminGenerationTableViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets a list of all reviewed pokemon.
        /// </summary>
        public List<ReviewedPokemon> ReviewedPokemon { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's dropdown menu.
        /// </summary>
        public DropdownViewModel DropdownViewModel { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
