using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent a pokemon's admin dropdown menu.
    /// </summary>
    public class AdminPokemonViewModel
    {
        /// <summary>
        /// Gets or sets the admin's dropdown.
        /// </summary>
        public DropdownViewModel DropdownViewModel { get; set; }

        /// <summary>
        /// Gets or sets the pokemon.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Gets or sets the Generation.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
