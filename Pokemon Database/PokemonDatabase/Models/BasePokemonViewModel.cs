using System;
using System.Collections.Generic;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.Models
{
    public class BasePokemonViewModel : Pokemon
    {
        public List<EggCycle> AllEggCycles { get; set; }
        public List<GenderRatio> AllGenderRatios { get; set; }
        public List<Classification> AllClassifications { get; set; }
        public List<Generation> AllGenerations { get; set; }
        public List<ExperienceGrowth> AllExperienceGrowths { get; set; }
        public List<CaptureRate> AllCaptureRates { get; set; }
        public List<BaseHappiness> AllBaseHappinesses { get; set; }
    }
}