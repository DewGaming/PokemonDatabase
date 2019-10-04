using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class FormItem   
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
        [Required]
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}