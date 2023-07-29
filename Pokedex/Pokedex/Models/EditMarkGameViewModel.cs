using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the edit mark game availability view model.
    /// </summary>
    public class EditMarkGameViewModel
    {
        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets a list of all marks.
        /// </summary>
        public List<Mark> AllMarks { get; set; }

        /// <summary>
        /// Gets or sets a list of all mark game details.
        /// </summary>
        public List<MarkGameDetail> MarkGameDetails { get; set; }
    }
}
