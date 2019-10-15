using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class BattleItem
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
        [Required, Display(Name = "Game(s) of Origin")]
        public string GenerationId { get; set; }
        public Generation Generation { get; set; }
        [Required, Display(Name = "Only In This Generation")]
        public bool OnlyInThisGeneration { get; set; }
        [ Display(Name = "Item For This Pokemon")]
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}