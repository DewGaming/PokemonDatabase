using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonFormDetail
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Pokemon's Form")]
        public int FormId { get; set; }
        public Form Form { get; set; }
        [Required]
        public int OriginalPokemonId { get; set; }
        public Pokemon OriginalPokemon { get; set; }
        [Required]
        public int AltFormPokemonId { get; set; }
        public Pokemon AltFormPokemon { get; set; }
    }
}