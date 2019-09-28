using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class UpdatePokemonListViewModel
    {
        public List<Pokemon> PokemonList { get; set; }

        public Generation Generation { get; set; }
    }
}