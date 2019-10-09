using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class CreatePokemonTeamViewModel : PokemonTeam
    {
        public List<Generation> AllGenerations { get; set; }
    }
}