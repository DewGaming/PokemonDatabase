using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class CreateTeamPokemonViewModel : PokemonTeamDetail
    {
        public List<Pokemon> AllPokemon { get; set; }

        public List<Nature> AllNatures { get; set; }

        public int? GameId { get; set; }
    }
}