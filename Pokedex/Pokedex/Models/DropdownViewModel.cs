using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the Admin Dropdown view model.
    /// </summary>
    public class DropdownViewModel
    {
        /// <summary>
        /// Gets or sets all the lists needed for admin commands.
        /// </summary>
        public AllAdminPokemonViewModel AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets the pokemon used for the dropdown.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets the application's configuration.
        /// </summary>
        public AppConfig AppConfig { get; set; }

        /// <summary>
        /// Gets or sets the generation.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
