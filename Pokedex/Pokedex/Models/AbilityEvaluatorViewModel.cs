using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ability evaluator page view model.
    /// </summary>
    public class AbilityEvaluatorViewModel
    {
        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public List<Ability> AllAbilities { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
