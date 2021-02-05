using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocation
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string Location { get; set; }
        [Required]
        public byte Rarity { get; set; }
        [Required]
        public byte MinLevel { get; set; }
        [Required]
        public byte MaxLevel { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}