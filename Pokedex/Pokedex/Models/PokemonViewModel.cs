using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class PokemonViewModel
    {
        public Pokemon Pokemon { get; set; }

        public Form Form { get; set; }

        public BaseStat BaseStats { get; set; }

        public EVYield EVYields { get; set; }

        public Type PrimaryType { get; set; }

        public Type SecondaryType { get; set; }

        public Ability PrimaryAbility { get; set; }

        public Ability SecondaryAbility { get; set; }

        public Ability HiddenAbility { get; set; }

        public EggGroup PrimaryEggGroup { get; set; }

        public EggGroup SecondaryEggGroup { get; set; }

        public Evolution PreEvolution { get; set; }

        public List<Evolution> Evolutions { get; set; }

        public TypeEffectivenessViewModel Effectiveness { get; set; }

        public List<Pokemon> SurroundingPokemon { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}