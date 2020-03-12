using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the battle item admin view model.
    /// </summary>
    public class BattleItemViewModel : BattleItem
    {
        /// <summary>
        /// Gets or sets a list of all battle items.
        /// </summary>
        public List<BattleItem> AllBattleItems { get; set; }

        /// <summary>
        /// Gets or sets a list of all generations.
        /// </summary>
        public List<Generation> AllGenerations { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon team details.
        /// </summary>
        public List<PokemonTeamDetail> AllPokemonTeamDetails { get; set; }
    }
}
