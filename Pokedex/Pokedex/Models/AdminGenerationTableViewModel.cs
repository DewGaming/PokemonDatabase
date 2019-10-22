using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AdminGenerationTableViewModel
    {
        public List<Pokemon> PokemonList { get; set; }

        public List<ReviewedPokemon> ReviewedPokemon { get; set; }

        public DropdownViewModel DropdownViewModel { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}