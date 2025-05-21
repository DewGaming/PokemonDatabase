using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Add, Update and Admin Form pages.
    /// </summary>
    public class FormViewModel
    {
        /// <summary>
        /// Gets or sets a list of all forms.
        /// </summary>
        public List<Form> AllForms { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon form details.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }
    }
}
