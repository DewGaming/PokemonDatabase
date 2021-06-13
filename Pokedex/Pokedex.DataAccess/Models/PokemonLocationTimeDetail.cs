using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocationTimeDetail
    {
        public int Id { get; set; }
        [Required]
        public int PokemonLocationDetailId { get; set; }
        public PokemonLocationDetail PokemonLocationDetail { get; set; }
        [Display(Name = "Time"), Required]
        public int TimeId { get; set; }
        public Time Time { get; set; }
    }
}