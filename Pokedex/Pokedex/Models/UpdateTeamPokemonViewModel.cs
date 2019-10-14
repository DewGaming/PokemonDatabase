using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class UpdateTeamPokemonViewModel
    {
        public PokemonTeamDetail PokemonTeamDetail { get; set; }

        public List<Pokemon> AllPokemon { get; set; }

        public List<Ability> AllAbilities { get; set; }
    }
}