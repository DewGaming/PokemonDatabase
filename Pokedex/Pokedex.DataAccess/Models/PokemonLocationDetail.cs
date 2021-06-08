using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocationDetail
    {
        public int Id { get; set; }
        public double ChanceOfEncounter { get; set; }
        [Required]
        public int MinimumLevel { get; set; }
        [Required]
        public int MaximumLevel { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int LocationId { get; set; }
        public Location Location { get; set; }
        [Required]
        public int CaptureMethodId { get; set; }
        public CaptureMethod CaptureMethod { get; set; }
        public int? TimeId { get; set; }
        public Time Time { get; set; }
        public int? SeasonId { get; set; }
        public Season Season { get; set; }
        public int? WeatherId { get; set; }
        public Weather Weather { get; set; }
        public bool SOSBattleOnly { get; set; }
    }
}