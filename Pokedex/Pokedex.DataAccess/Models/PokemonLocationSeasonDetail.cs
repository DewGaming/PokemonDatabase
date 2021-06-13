using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocationSeasonDetail
    {
        public int Id { get; set; }
        [Required]
        public int PokemonLocationDetailId { get; set; }
        public PokemonLocationDetail PokemonLocationDetail { get; set; }
        [Display(Name = "Season"), Required]
        public int SeasonId { get; set; }
        public Season Season { get; set; }
    }
}