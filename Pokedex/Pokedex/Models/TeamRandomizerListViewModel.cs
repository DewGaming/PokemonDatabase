using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon team randomizer lists view model for the randomizer page.
    /// </summary>
    public class TeamRandomizerListViewModel
    {
        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all generations.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }

        /// <summary>
        /// Gets or sets a list of all types.
        /// </summary>
        public List<Type> AllTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all special groupings.
        /// </summary>
        public List<SpecialGrouping> AllSpecialGroupings { get; set; }

        /// <summary>
        /// Gets or sets a list of all form groups.
        /// </summary>
        public List<FormGroup> AllFormGroups { get; set; }

        /// <summary>
        /// Gets or sets a list of all form group game details.
        /// </summary>
        public List<FormGroupGameDetail> AllFormGroupGameDetails { get; set; }

        /// <summary>
        /// Gets or sets the number of incomplete pokemon.
        /// </summary>
        public int IncompleteCount { get; set; }
    }
}
