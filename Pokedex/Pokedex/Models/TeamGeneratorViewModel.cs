using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TeamGeneratorViewModel
    {
        public List<Pokemon> AllPokemonChangedNames { get; set; }

        public List<Pokemon> AllPokemonOriginalNames { get; set; }

        public List<string> PokemonURLs { get; set; }
    }
}