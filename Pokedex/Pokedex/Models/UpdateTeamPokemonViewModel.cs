using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the update pokemon team's pokemon view model.
    /// </summary>
    public class UpdateTeamPokemonViewModel
    {
        /// <summary>
        /// Gets or sets the pokemon team's pokemon that is being updated.
        /// </summary>
        public PokemonTeamDetail PokemonTeamDetail { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon natures.
        /// </summary>
        public List<Nature> AllNatures { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon types.
        /// </summary>
        public List<Type> AllTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon abilities.
        /// </summary>
        public List<Ability> AllAbilities { get; set; }

        /// <summary>
        /// Gets or sets a list of all battle items.
        /// </summary>
        public List<BattleItem> AllBattleItems { get; set; }

        /// <summary>
        /// Gets or sets the pokemon team's game id.
        /// </summary>
        public int? GameId { get; set; }

        /// <summary>
        /// Gets a list of possible genders.
        /// </summary>
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
