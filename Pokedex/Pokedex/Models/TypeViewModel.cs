using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class TypeViewModel
    {
        public List<Type> AllTypes { get; set; }
        public List<PokemonTypeDetail> AllPokemon { get; set; }
    }
}