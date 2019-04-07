using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class Pokemon   
    {
        [StringLength(6)]
        public string Id { get; set; }
        [StringLength(25)]
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Height { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public int ExpYield { get; set; }
        [Required]
        public bool IsArchived { get; set; }
        [Required]
        public EggCycle EggCycle { get; set; }
        [Required]
        public GenderRatio GenderRatio { get; set; }
        [Required]
        public Classification Classification { get; set; }
        [StringLength(20)]
        [Required]
        public Generation Generation { get; set; }
        [Required]
        public ExperienceGrowth ExperienceGrowth { get; set; }
        [Required]
        public CaptureRate CaptureRate { get; set; }
        [Required]
        public BaseHappiness BaseHappiness { get; set; }
    }
}