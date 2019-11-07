using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonGameDetail  
    {
        public int Id { get; set; }
        [Required]
        public string GenerationId { get; set; }
        public Generation Generation { get; set; }
        [Required]
        public string PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}