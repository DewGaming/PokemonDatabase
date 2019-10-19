using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class BattleItemViewModel : BattleItem
    {
        public List<BattleItem> AllBattleItems { get; set; }

        public List<Generation> AllGenerations { get; set; }

        public List<Pokemon> AllPokemon { get; set; }

        public List<PokemonTeamDetail> AllPokemonTeamDetails { get; set; }
    }
}