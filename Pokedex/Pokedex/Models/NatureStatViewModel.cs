using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Nature page.
    /// </summary>
    public class NatureStatViewModel : Nature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NatureStatViewModel"/> class.
        /// </summary>
        public NatureStatViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NatureStatViewModel"/> class.
        /// </summary>
        /// <param name="nature">The shiny hunt.</param>
        public NatureStatViewModel(Nature nature)
            : base(nature)
        {
        }

        /// <summary>
        /// Gets or sets a list of all stats.
        /// </summary>
        public List<Stat> AllStats { get; set; }
    }
}
