using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Create, Update, and Delete team page view model.
    /// </summary>
    public class PokemonTeamViewModel : PokemonTeam
    {
        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
