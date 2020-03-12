using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TypingEvaluatorViewModel
    {
        public List<PokemonTypeDetail> AllPokemonWithTypes { get; set; }

        public List<Pokemon> AllPokemon { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}
