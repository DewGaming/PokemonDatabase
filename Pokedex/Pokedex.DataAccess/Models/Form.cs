using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    /// <summary>
    /// The class that is used to represent the form table in a database.
    /// </summary>
    public class Form
    {
        /// <summary>
        /// Gets or sets the primary key of the table.
        /// </summary>
        public int Id { get; set; }

        [StringLength(25), Required]
        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        public string Name { get; set; }

        [Display(Name="Part Of \"Other Forms\" In Randomize Team Page"), Required]
        /// <summary>
        /// Gets or sets the ability to appear in the other forms category in the randomize team page.
        /// </summary>
        public bool Randomizable { get; set; }
    }
}