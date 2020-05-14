using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Update Pokemon ajax call.
    /// </summary>
    public class GenerationTableViewModel
    {
        /// <summary>
        /// Gets or sets a list of pokemon type details.
        /// </summary>
        public List<PokemonTypeDetail> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets a list of pokemon without type details.
        /// </summary>
        public List<Pokemon> PokemonNoTypeList { get; set; }

        /// <summary>
        /// Gets or sets a list of pokemon that are alternate forms.
        /// </summary>
        public List<Pokemon> AltFormsList { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
