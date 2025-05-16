using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the serach's view model.
    /// </summary>
    public class AllPokemonTypeViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon type details.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all alt form pokemon.
        /// </summary>
        public List<PokemonFormDetail> AllAltForms { get; set; }

        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
