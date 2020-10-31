using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonCaptureRateDetail  
    {
        public int Id { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required, Display(Name = "Capture Rate")]
        public int CaptureRateId { get; set; }
        public CaptureRate CaptureRate { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}