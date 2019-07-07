using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ShinyHunt
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Hunt Technique Used")]
        public int? ShinyHuntingTechniqueId { get; set; }
        public ShinyHuntingTechnique ShinyHuntingTechnique { get; set; }
        [Required]
        [Display(Name = "Pokemon Being Hunted")]
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int? UserId { get; set; }
        public User User { get; set; }
        [Required]
        [Display(Name = "Game(s) Hunting In")]
        public string GenerationId { get; set; }
        public Generation Generation { get; set; }
        [Required]
        public int ShinyAttemptCount { get; set; }
        [Required]
        [Display(Name = "Using Shiny Charm")]
        public bool HasShinyCharm { get; set; }
        [Required]
        public bool IsPokemonCaught { get; set; }
        [Required]
        public bool HuntComplete { get; set; }
        [Required]
        public bool HuntRetried { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}