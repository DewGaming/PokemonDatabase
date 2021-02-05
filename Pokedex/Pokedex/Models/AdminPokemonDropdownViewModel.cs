using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the pokemon dropdown view model.
    /// </summary>
    public class AdminPokemonDropdownViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<PokemonViewModel> PokemonList { get; set; }

        /// <summary>
        /// Gets or sets the admin generation dropdown menu.
        /// </summary>
        public AdminGenerationTableViewModel AdminDropdown { get; set; }

        /// <summary>
        /// Gets or sets the pokemon's id that will be selected upon opening the page.
        /// </summary>
        public int PokemonId { get; set; }

        /// <summary>
        /// Gets or sets the generation's id that will be selected upon opening the page.
        /// </summary>
        public int GenerationId { get; set; }
    }
}
