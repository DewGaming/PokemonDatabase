using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location weather view model.
    /// </summary>
    public class PokemonLocationWeatherDetailViewModel
    {
        /// <summary>
        /// Gets or sets a list of all weathers.
        /// </summary>
        public List<Weather> AllWeathers { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon locaiton detail.
        /// </summary>
        public PokemonLocationDetail PokemonLocationDetail { get; set; }

        /// <summary>
        /// Gets or sets the selected weather id.
        /// </summary>
        public List<int> WeatherIds { get; set; }
    }
}
