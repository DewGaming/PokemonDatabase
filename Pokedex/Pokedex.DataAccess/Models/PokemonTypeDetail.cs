using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTypeDetail  
    {
        public int Id { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Display(Name = "Primary Type"), Required]
        public int? PrimaryTypeId { get; set; }
        public Type PrimaryType { get; set; }
        [Display(Name = "Secondary Type")]
        public int? SecondaryTypeId { get; set; }
        public Type SecondaryType { get; set; }
        public int? GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}