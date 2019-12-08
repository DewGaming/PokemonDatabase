using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class GenerationTableViewModel
    {
        public List<PokemonTypeDetail> PokemonList { get; set; }
        
        public List<Pokemon> PokemonNoTypeList { get; set; }

        public bool ShowSprites { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}