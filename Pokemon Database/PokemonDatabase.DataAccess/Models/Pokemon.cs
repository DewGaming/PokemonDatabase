using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class Pokemon   
    {
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
        public virtual EggCycle EggCycle { get; set; }
        [Required]
        public virtual GenderRatio GenderRatio { get; set; }
        [Required]
        public virtual Classification Classification { get; set; }
        [StringLength(20)]
        [Required]
        public virtual Generation Generation { get; set; }
        [Required]
        public virtual ExperienceGrowth ExperienceGrowth { get; set; }
        [Required]
        public virtual CaptureRate CaptureRate { get; set; }
        [Required]
        public virtual BaseHappiness BaseHappiness { get; set; }
    }
}