using System;

namespace PokemonDatabase.Models
{
    public class Pokemon   
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public int ExpYield { get; set; }
        public int EggCycleID { get; set; }
        public virtual EggCycle EggCycle { get; set; }
        public int GenderRatioID { get; set; }
        public virtual GenderRatio GenderRatio { get; set; }
        public int ClassificationID { get; set; }
        public virtual Classification Classification { get; set; }
        public string GenerationID { get; set; }
        public virtual Generation Generation { get; set; }
        public int ExperienceGrowthID { get; set; }
        public virtual ExperienceGrowth ExperienceGrowth { get; set; }
        public int CaptureRateID { get; set; }
        public virtual CaptureRate CaptureRate { get; set; }
        public int BaseHappinessID { get; set; }
        public virtual BaseHappiness BaseHappiness { get; set; }
    }
}