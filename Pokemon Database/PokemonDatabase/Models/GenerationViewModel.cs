using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class GenerationViewModel
    {
        public List<Generation> AllGenerations { get; set; }
        public List<Pokemon> AllPokemon { get; set; }
    }
}