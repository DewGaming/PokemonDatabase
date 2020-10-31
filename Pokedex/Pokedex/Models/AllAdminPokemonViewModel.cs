using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent all the lists needed for admin commands.
    /// </summary>
    public class AllAdminPokemonViewModel
    {
        /// <summary>
        /// Gets or sets a list of all pokemon.
        /// </summary>
        public List<Pokemon> AllPokemon { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon type details.
        /// </summary>
        public List<PokemonTypeDetail> AllTypings { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon ability details.
        /// </summary>
        public List<PokemonAbilityDetail> AllAbilities { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon egg groups details.
        /// </summary>
        public List<PokemonEggGroupDetail> AllEggGroups { get; set; }

        /// <summary>
        /// Gets or sets a list of all base stats.
        /// </summary>
        public List<BaseStat> AllBaseStats { get; set; }

        /// <summary>
        /// Gets or sets a list of all EV yields.
        /// </summary>
        public List<EVYield> AllEVYields { get; set; }

        /// <summary>
        /// Gets or sets a list of all alternate forms.
        /// </summary>
        public List<PokemonFormDetail> AllAltForms { get; set; }

        /// <summary>
        /// Gets or sets a list of all evolutions.
        /// </summary>
        public List<Evolution> AllEvolutions { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon legendary details.
        /// </summary>
        public List<PokemonLegendaryDetail> AllLegendaryDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of all pokemon capture rates.
        /// </summary>
        public List<PokemonCaptureRateDetail> AllPokemonCaptureRates { get; set; }
    }
}
