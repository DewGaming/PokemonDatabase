using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class TypeViewModel
    {
        public List<Type> AllTypes { get; set; }
        public List<PokemonTypeDetail> AllPokemon { get; set; }
    }
}