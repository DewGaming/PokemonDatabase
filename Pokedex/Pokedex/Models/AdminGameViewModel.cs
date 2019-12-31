using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AdminGameViewModel
    {
        public List<Game> AllGames { get; set; }

        public List<Pokemon> AllPokemon { get; set; }
    }
}