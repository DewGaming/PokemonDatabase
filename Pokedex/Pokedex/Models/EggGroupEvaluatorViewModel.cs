using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EggGroupEvaluatorViewModel
    {
        public List<PokemonEggGroupDetail> AllPokemonWithEggGroups { get; set; }

        public List<Pokemon> AllPokemon { get; set; }

        public List<Pokemon> AllOriginalPokemon { get; set; }

        public AppConfig AppConfig { get; set; }

        public Pokemon SearchedPokemon { get; set; }

        public List<EggGroup> PokemonEggGroups { get; set; }
    }
}