using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon location season view model.
    /// </summary>
    public class PokemonLocationSeasonDetailViewModel : PokemonLocationSeasonDetail
    {
        /// <summary>
        /// Gets or sets a list of all seasons.
        /// </summary>
        public List<Season> AllSeasons { get; set; }
    }
}
