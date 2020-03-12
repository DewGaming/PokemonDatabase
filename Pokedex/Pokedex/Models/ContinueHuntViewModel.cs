using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the continue hunting page view model.
    /// </summary>
    public class ContinueHuntViewModel
    {
        /// <summary>
        /// Gets or sets the shiny hunt.
        /// </summary>
        public ShinyHunt ShinyHunt { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the shiny hunt index.
        /// </summary>
        public int HuntIndex { get; set; }
    }
}
