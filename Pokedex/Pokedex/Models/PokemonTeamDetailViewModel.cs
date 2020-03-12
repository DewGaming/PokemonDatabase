using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the view model for the Delete Pokemon Team Pokemon page.
    /// </summary>
    public class PokemonTeamDetailViewModel : PokemonTeamDetail
    {
        /// <summary>
        /// Gets or sets the pokemon's evs.
        /// </summary>
        public PokemonTeamEV EVs { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's ivs.
        /// </summary>
        public PokemonTeamIV IVs { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's moveset.
        /// </summary>
        public PokemonTeamMoveset Moveset { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
