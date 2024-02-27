using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the shiny dex view model.
    /// </summary>
    public class PokemonShinyHuntDetails
    {
        /// <summary>
        /// Gets or sets a pokemon id.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the pokemon was captured.
        /// </summary>
        public bool IsCaptured { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the pokemon is an alternate form.
        /// </summary>
        public bool IsAltForm { get; set; }
    }
}
