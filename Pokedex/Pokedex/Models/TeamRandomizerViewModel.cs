using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TeamRandomizerViewModel
    {
        public List<Pokemon> AllPokemonChangedNames { get; set; }

        public List<Pokemon> AllPokemonOriginalNames { get; set; }

        public List<Ability> PokemonAbilities { get; set; }

        public List<string> PokemonURLs { get; set; }

        public string ExportString { get; set; }
    }
}