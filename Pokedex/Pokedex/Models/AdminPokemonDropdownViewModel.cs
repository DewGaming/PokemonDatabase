using System.Collections.Generic;

namespace Pokedex.Models
{
    public class AdminPokemonDropdownViewModel
    {
        public List<PokemonViewModel> PokemonList { get; set; }

        public AdminGenerationTableViewModel AdminDropdown { get; set; }
    }
}