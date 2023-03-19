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
        /// Gets or sets the application's config.
        /// </summary>
        public AppConfig AppConfig { get; set; }
    }
}
