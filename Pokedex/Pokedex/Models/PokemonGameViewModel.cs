using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonGameViewModel
    {
        public Pokemon Pokemon { get; set; }

        public List<PokemonGameDetail> PokemonGameDetails { get; set; }

        public List<Game> AllGames { get; set; }
    }
}