using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class UpdateTeamPokemonViewModel
    {
        public PokemonTeamDetail PokemonTeamDetail { get; set; }

        public List<Pokemon> AllPokemon { get; set; }

        public List<Nature> AllNatures { get; set; }

        public List<Ability> AllAbilities { get; set; }

        public List<BattleItem> AllBattleItems { get; set; }

        public string GenerationId { get; set; }

        public List<string> Genders
        {
            get
            {
                return new List<string>()
                {
                    "Male",
                    "Female",
                };
            }
        }
    }
}