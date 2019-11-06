using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ReviewedPokemon   
    {
        public int Id { get; set; }
        [Display(Name = "Pokemon Id"), StringLength(6), Required]
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}