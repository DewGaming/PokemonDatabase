using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the view model to create pokemon team pokemon.
    /// </summary>
    public class CreateTeamPokemonViewModel : PokemonTeamDetail
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all natures.
        /// </summary>
        public List<Nature> AllNatures { get; set; }

        /// <summary>
        /// Gets or sets the team's game id, if applicable.
        /// </summary>
        public int? GameId { get; set; }
    }
}
