using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location weather view model.
    /// </summary>
    public class PokemonLocationWeatherDetailViewModel : PokemonLocationWeatherDetail
    {
        /// <summary>
        /// Gets or sets a list of all weathers.
        /// </summary>
        public List<Weather> AllWeathers { get; set; }
    }
}
