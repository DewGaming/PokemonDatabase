using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ReviewedPokemon
    {
        public int Id { get; set; }
        [Display(Name = "Pokemon Id"), Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}