using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon teams view model.
    /// </summary>
    public class PokemonTeamsViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon teams.
        /// </summary>
        public List<PokemonTeam> AllPokemonTeams { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
