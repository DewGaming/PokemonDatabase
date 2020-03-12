using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon team moveset view model.
    /// </summary>
    public class PokemonTeamMovesetViewModel : PokemonTeamMoveset
    {
        /// <summary>
        /// Gets or sets the pokemon's id.
        /// </summary>
        public int PokemonId { get; set; }
    }
}
