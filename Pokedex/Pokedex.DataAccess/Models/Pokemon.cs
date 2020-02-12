using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class Pokemon   
    {
        public int Id { get; set; }
        [Display(Name = "Pokedex Number")]
        public int PokedexNumber { get; set; }
        [Display(Name = "Pokemon Name"), StringLength(25), Required]
        public string Name { get; set; }
        [Required, Column(TypeName = "decimal(4,1)")]
        public decimal Height { get; set; }
        [Required, Column(TypeName = "decimal(4,1)")]
        public decimal Weight { get; set; }
        [Display(Name = "Experience Yield"), Required]
        public int ExpYield { get; set; }
        [Required]
        public bool IsComplete { get; set; }
        [Display(Name = "Egg Cycle Count"), Required]
        public int EggCycleId { get; set; }
        public EggCycle EggCycle { get; set; }
        [Display(Name = "Gender Ratios"), Required]
        public int GenderRatioId { get; set; }
        public GenderRatio GenderRatio { get; set; }
        [Display(Name = "Classification"), Required]
        public int ClassificationId { get; set; }
        public Classification Classification { get; set; }
        [Display(Name = "Game(s) of Origin"), Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Display(Name = "Experience Growth"), Required]
        public int ExperienceGrowthId { get; set; }
        public ExperienceGrowth ExperienceGrowth { get; set; }
        [Display(Name = "Capture Rate"), Required]
        public int CaptureRateId { get; set; }
        public CaptureRate CaptureRate { get; set; }
        [Display(Name = "Base Happiness"), Required]
        public int BaseHappinessId { get; set; }
        public BaseHappiness BaseHappiness { get; set; }
    }
}