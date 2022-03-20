using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon team radio lists view model for the randomizer page.
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
        /// Gets or sets a list of all legendary types.
        /// </summary>
        public List<LegendaryType> AllLegendaryTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all forms.
        /// </summary>
        public List<FormGroup> AllFormGroups { get; set; }
    }
}
