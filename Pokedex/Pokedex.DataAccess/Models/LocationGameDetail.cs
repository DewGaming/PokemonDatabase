using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class LocationGameDetail
    {
        public int Id { get; set; }
        [Required]
        public int PokemonLocationDetailId { get; set; }
        public PokemonLocationDetail PokemonLocationDetail { get; set; }
        [Display(Name = "Game"), Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}