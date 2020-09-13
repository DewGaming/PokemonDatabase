using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add, Update, and Admin Move pages.
    /// </summary>
    public class MoveViewModel : Move
    {
        /// <summary>
        /// Gets or sets a list of all types.
        /// </summary>
        public List<Type> AllTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }

        /// <summary>
        /// Gets or sets a list of all move categories.
        /// </summary>
        public List<MoveCategory> AllMoveCategories { get; set; }
    }
}
