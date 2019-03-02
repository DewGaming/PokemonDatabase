using System;

namespace PokemonDatabase.Models
{
    public class PokemonViewModel
    {
        public Pokemon pokemon { get; set; }
        public BaseStat baseStats { get; set; }
        public EVYield evYields { get; set; }
    }
}