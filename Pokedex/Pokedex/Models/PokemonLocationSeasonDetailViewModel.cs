using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location season view model.
    /// </summary>
    public class PokemonLocationSeasonDetailViewModel
    {
        /// <summary>
        /// Gets or sets a list of all seasons.
        /// </summary>
        public List<Season> AllSeasons { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon locaiton detail.
        /// </summary>
        public PokemonLocationDetail PokemonLocationDetail { get; set; }

        /// <summary>
        /// Gets or sets the selected season id.
        /// </summary>
        public List<int> SeasonIds { get; set; }
    }
}
