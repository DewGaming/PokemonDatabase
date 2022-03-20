using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Create, Update, and Delete team page view model.
    /// </summary>
    public class FormModelViewModel : Form
    {
        /// <summary>
        /// Gets or sets the list of form groups.
        /// </summary>
        public List<FormGroup> AllFormGroups { get; set; }
    }
}
