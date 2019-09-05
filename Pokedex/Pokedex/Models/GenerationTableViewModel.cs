using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class GenerationTableViewModel
    {
        public List<PokemonTypeDetail> PokemonList { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}