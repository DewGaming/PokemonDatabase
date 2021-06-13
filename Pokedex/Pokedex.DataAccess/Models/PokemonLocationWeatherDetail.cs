using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocationWeatherDetail
    {
        public int Id { get; set; }
        [Required]
        public int PokemonLocationDetailId { get; set; }
        public PokemonLocationDetail PokemonLocationDetail { get; set; }
        [Display(Name = "Weather"), Required]
        public int WeatherId { get; set; }
        public Weather Weather { get; set; }
    }
}