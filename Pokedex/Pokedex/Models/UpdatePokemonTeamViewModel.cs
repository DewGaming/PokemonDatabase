using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class UpdatePokemonTeamViewModel : PokemonTeam
    {
        public List<Generation> AllGenerations { get; set; }
    }
}