using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the typing evaluator page view model.
    /// </summary>
    public class AbilityEvaluatorPageViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon ability details.
        /// </summary>
        public List<PokemonAbilityDetail> AllPokemonWithAbility { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the generation id.
        /// </summary>
        public int GenerationId { get; set; }

        /// <summary>
        /// Gets or sets the ability.
        /// </summary>
        public Ability Ability { get; set; }
    }
}
