using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the typing evaluator page view model.
    /// </summary>
    public class AbilityEvaluatorViewModel
    {
        /// <summary>
        /// Gets or sets a list of all abilities.
        /// </summary>
        public List<Ability> AllAbilities { get; set; }

        /// <summary>
        /// Gets or sets a list of all generations.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }
    }
}
