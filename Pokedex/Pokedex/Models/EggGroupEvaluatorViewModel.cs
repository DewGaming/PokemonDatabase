using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Egg Group Evaluator page view model.
    /// </summary>
    public class EggGroupEvaluatorViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon egg group details.
        /// </summary>
        public List<PokemonEggGroupDetail> AllPokemonWithEggGroups { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon without changes.
        /// </summary>
        public List<Pokemon> AllOriginalPokemon { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets pokemon being searched.
        /// </summary>
        public Pokemon SearchedPokemon { get; set; }

        /// <summary>
        /// Gets or sets game being searched in.
        /// </summary>
        public Game SearchedGame { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon egg groups.
        /// </summary>
        public List<EggGroup> PokemonEggGroups { get; set; }

        /// <summary>
        /// Gets or sets the generation.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
