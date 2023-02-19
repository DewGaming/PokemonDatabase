using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ability view model.
    /// </summary>
    public class PokemonImageViewModel : Pokemon
    {
        /// <summary>
        /// Gets or sets a value indicating whether a pokemon has a shiny image.
        /// </summary>
        public bool HasShiny { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a pokemon has a home image.
        /// </summary>
        public bool HasHome { get; set; }

        /// <summary>
        /// Gets or sets the current image being viewed.
        /// </summary>
        public string CurrentImage { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets a list of the pokemon's new type.
        /// </summary>
        public Type Type { get; set; }
    }
}
