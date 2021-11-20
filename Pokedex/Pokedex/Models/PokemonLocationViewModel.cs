using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon dropdown view model.
    /// </summary>
    public class PokemonLocationViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's location game details.
        /// </summary>
        public List<PokemonLocationGameDetail> PokemonLocations { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's location time details.
        /// </summary>
        public List<PokemonLocationTimeDetail> PokemonTimes { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's location season details.
        /// </summary>
        public List<PokemonLocationSeasonDetail> PokemonSeasons { get; set; }

        /// <summary>
        /// Gets or sets the games this pokemon is available in.
        /// </summary>
        public List<Game> GamesAvailableIn { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's id that will be selected upon opening the page.
        /// </summary>
        public int PokemonId { get; set; }

        /// <summary>
        /// Gets or sets the generation's id that will be selected upon opening the page.
        /// </summary>
        public int GenerationId { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
