using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the shiny dex view model.
    /// </summary>
    public class ShinyDexViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonShinyHuntDetails> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
