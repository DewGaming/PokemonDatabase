using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocationDetail
    {
        public int Id { get; set; }
        [Display(Name = "Chance of Encounter")]
        public double ChanceOfEncounter { get; set; }
        [Display(Name = "Minimum Level"), Required]
        public int MinimumLevel { get; set; }
        [Display(Name = "Maximum Level"), Required]
        public int MaximumLevel { get; set; }
        [Display(Name = "Pokemon"), Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int LocationId { get; set; }
        public Location Location { get; set; }
        [Display(Name = "Capture Method"), Required]
        public int CaptureMethodId { get; set; }
        public CaptureMethod CaptureMethod { get; set; }
        [Display(Name = "Only Encounterable Thru SOS Battles")]
        public bool SOSBattleOnly { get; set; }
        [Display(Name = "Only Available Thru Special Spawns")]
        public bool SpecialSpawn { get; set; }
    }
}