using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonTeamsViewModel
    {
        public List<PokemonTeam> AllPokemonTeams { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}