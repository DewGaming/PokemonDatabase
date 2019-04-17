using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class ClassificationViewModel
    {
        public List<Classification> AllClassifications { get; set; }
        public List<Pokemon> AllPokemon { get; set; }
    }
}