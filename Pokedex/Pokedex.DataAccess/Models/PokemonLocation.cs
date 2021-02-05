using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonLocation
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string HowToObtain { get; set; }
        public string Condition { get; set; }
        public byte Rarity { get; set; }
        public byte MinLevel { get; set; }
        public byte MaxLevel { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}