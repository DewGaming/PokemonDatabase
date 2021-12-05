using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the admin pokemon location view model.
    /// </summary>
    public class PokemonLocationDetailAdminViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonLocationDetail> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon location game details.
        /// </summary>
        public List<PokemonLocationGameDetail> AllPokemonLocationGameDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon location season details.
        /// </summary>
        public List<PokemonLocationSeasonDetail> AllPokemonLocationSeasonDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon location time details.
        /// </summary>
        public List<PokemonLocationTimeDetail> AllPokemonLocationTimeDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon location weather details.
        /// </summary>
        public List<PokemonLocationWeatherDetail> AllPokemonLocationWeatherDetails { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Location Location { get; set; }
    }
}
