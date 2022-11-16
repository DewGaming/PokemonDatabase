using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class GameStarterDetail
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}