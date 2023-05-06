using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the delete shiny hunt view model.
    /// </summary>
    public class DeleteShinyHuntViewModel : ShinyHunt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteShinyHuntViewModel"/> class.
        /// </summary>
        /// <param name="shinyHunt">The shiny hunt.</param>
        public DeleteShinyHuntViewModel(ShinyHunt shinyHunt)
            : base(shinyHunt)
        {
        }

        /// <summary>
        /// Gets or sets the application's config.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
