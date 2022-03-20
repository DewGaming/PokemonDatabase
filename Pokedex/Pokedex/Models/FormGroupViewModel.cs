using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the ability view model.
    /// </summary>
    public class FormGroupViewModel
    {
        /// <summary>
        /// Gets or sets a list of all form groups.
        /// </summary>
        public List<FormGroup> AllFormGroups { get; set; }

        /// <summary>
        /// Gets or sets a list of all form.
        /// </summary>
        public List<Form> AllForms { get; set; }
    }
}
