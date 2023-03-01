using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the typing evaluator page view model.
    /// </summary>
    public class TypingEvaluatorViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon type details.
        /// </summary>
        public List<PokemonTypeDetail> AllPokemonWithTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the game's id.
        /// </summary>
        public Game Game { get; set; }
    }
}
