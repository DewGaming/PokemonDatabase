using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AllPokemonTypeViewModel
    {
        public List<PokemonTypeDetail> AllPokemon { get; set; }

        public bool SlowConnection { get; set; }
    }
}