using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class ClassificationViewModel
    {
        public List<Classification> AllClassifications { get; set; }
        public List<Pokemon> AllPokemon { get; set; }
    }
}