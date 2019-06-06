using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AllAdminPokemonViewModel
    {
        public List<Pokemon> AllPokemon { get; set; }

        public List<PokemonFormDetail> AllAltForms { get; set; }

        public List<Evolution> AllEvolutions { get; set; }

        public bool SlowConnection { get; set; }
    }
}