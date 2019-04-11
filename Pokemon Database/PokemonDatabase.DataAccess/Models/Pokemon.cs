using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class Pokemon   
    {
        [Display(Name = "Pokedex Number"), StringLength(6)]
        public string Id { get; set; }
        [Display(Name = "Pokemon Name"), StringLength(25), Required]
        public string Name { get; set; }
        [Required]
        public decimal Height { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Display(Name = "Experience Yield"), Required]
        public int ExpYield { get; set; }
        [Required]
        public bool IsArchived { get; set; }
        [Display(Name = "Egg Cycle Count"), Required]
        public int? EggCycleId { get; set; }
        public EggCycle EggCycle { get; set; }
        [Display(Name = "Gender Ratios"), Required]
        public int? GenderRatioId { get; set; }
        public GenderRatio GenderRatio { get; set; }
        [Display(Name = "Classification"), Required]
        public int? ClassificationId { get; set; }
        public Classification Classification { get; set; }
        [Display(Name = "Game(s) of Origin"), StringLength(20), Required]
        public string GenerationId { get; set; }
        public Generation Generation { get; set; }
        [Display(Name = "Experience Growth"), Required]
        public int? ExperienceGrowthId { get; set; }
        public ExperienceGrowth ExperienceGrowth { get; set; }
        [Display(Name = "Capture Rate"), Required]
        public int? CaptureRateId { get; set; }
        public CaptureRate CaptureRate { get; set; }
        [Display(Name = "Base Happiness"), Required]
        public int? BaseHappinessId { get; set; }
        public BaseHappiness BaseHappiness { get; set; }
    }
}