using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon team view model for the randomizer page.
    /// </summary>
    public class TeamRandomizerViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon with updated names.
        /// </summary>
        public List<Pokemon> AllPokemonChangedNames { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon without updated names.
        /// </summary>
        public List<Pokemon> AllPokemonOriginalNames { get; set; }

        /// <summary>
        /// Gets or sets a list of abilities.
        /// </summary>
        public List<Ability> PokemonAbilities { get; set; }

        /// <summary>
        /// Gets or sets a list of URLs for pokemon.
        /// </summary>
        public List<string> PokemonURLs { get; set; }

        /// <summary>
        /// Gets or sets the applicaiton configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the pokemon team's export string.
        /// </summary>
        public string ExportString { get; set; }
    }
}
