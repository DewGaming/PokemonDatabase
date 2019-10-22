using System.Collections.Generic;

namespace Pokedex.Models
{
    public class AdminPokemonViewModel
    {
        public List<PokemonViewModel> PokemonList { get; set; }

        public AdminGenerationTableViewModel AdminDropdown { get; set; }
    }
}