using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokeballGameDetail
    {
        public int Id { get; set; }
        [Required]
        public int PokeballId { get; set; }
        public Pokeball Pokeball { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}