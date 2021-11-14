using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location view model.
    /// </summary>
    public class PokemonLocationDetailViewModel : PokemonLocationDetail
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all capture methods.
        /// </summary>
        public List<CaptureMethod> AllCaptureMethods { get; set; }

        /// <summary>
        /// Gets or sets a list of all times.
        /// </summary>
        public List<Time> AllTimes { get; set; }

        /// <summary>
        /// Gets or sets a list of all seasons.
        /// </summary>
        public List<Season> AllSeasons { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public Region Region { get; set; }
    }
}
