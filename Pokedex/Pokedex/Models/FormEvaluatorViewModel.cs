using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Form Evaluator page view model.
    /// </summary>
    public class FormEvaluatorViewModel
    {
        /// <summary>
        /// Gets or sets a list of all alternate form pokemon.
        /// </summary>
        public List<PokemonFormDetail> AllAltFormPokemon { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the generation id.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
