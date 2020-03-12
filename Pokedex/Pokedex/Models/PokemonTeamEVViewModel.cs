using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon team ev view model.
    /// </summary>
    public class PokemonTeamEVViewModel : PokemonTeamEV
    {
        /// <summary>
        /// Gets or sets the pokemon's id.
        /// </summary>
        public int PokemonId { get; set; }
    }
}
