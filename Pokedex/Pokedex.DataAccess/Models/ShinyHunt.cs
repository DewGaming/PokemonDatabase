using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ShinyHunt
    {
        public int Id { get; set; }
        [Required]
        public int? ShinyHuntingTechniqueId { get; set; }
        public ShinyHuntingTechnique ShinyHuntingTechnique { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int? UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int ShinyAttemptCount { get; set; }
        [Required]
        public bool HasShinyCharm { get; set; }
        [Required]
        public bool IsPokemonCaught { get; set; }
        [Required]
        public bool HuntComplete { get; set; }
    }
}