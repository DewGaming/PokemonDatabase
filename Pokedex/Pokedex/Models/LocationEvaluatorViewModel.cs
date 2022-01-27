using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the locations evaluator view model.
    /// </summary>
    public class LocationEvaluatorViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon's location game details.
        /// </summary>
        public List<PokemonLocationGameDetail> PokemonLocationGames { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's location time details.
        /// </summary>
        public List<PokemonLocationTimeDetail> PokemonTimes { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's location season details.
        /// </summary>
        public List<PokemonLocationSeasonDetail> PokemonSeasons { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's location weather details.
        /// </summary>
        public List<PokemonLocationWeatherDetail> PokemonWeathers { get; set; }

        /// <summary>
        /// Gets or sets the location that will be selected upon opening the page.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the game that will be selected upon opening the page.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
