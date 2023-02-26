using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ability view model.
    /// </summary>
    public class FormGroupGameDetailViewModel
    {
        /// <summary>
        /// Gets or sets the form group.
        /// </summary>
        public FormGroup FormGroup { get; set; }

        /// <summary>
        /// Gets or sets a list of all form group game details.
        /// </summary>
        public List<FormGroupGameDetail> AllFormGroupGameDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all games.
        /// </summary>
        public List<Game> AllGames { get; set; }
    }
}
