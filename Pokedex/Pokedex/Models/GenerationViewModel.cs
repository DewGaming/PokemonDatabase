using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Generation page.
    /// </summary>
    public class GenerationViewModel
    {
        /// <summary>
        /// Gets or sets a list of all generations.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
