using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Move page view model.
    /// </summary>
    public class AdminMoveViewModel
    {
        /// <summary>
        /// Gets or sets a list of all moves.
        /// </summary>
        public List<Move> AllMoves { get; set; }

        /// <summary>
        /// Gets or sets a list of all types.
        /// </summary>
        public List<Type> AllTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all move categories.
        /// </summary>
        public List<MoveCategory> AllMoveCategories { get; set; }
    }
}
