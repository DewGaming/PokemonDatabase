using System;
using System.Collections.Generic;

namespace PokemonDatabase.Models
{
    public class PokemonViewModel
    {
        public Pokemon pokemon { get; set; }
        public BaseStat baseStats { get; set; }
        public EVYield evYields { get; set; }
        public List<Models.Type> types { get; set; }
        public List<Ability> abilities { get; set; }
    }
}