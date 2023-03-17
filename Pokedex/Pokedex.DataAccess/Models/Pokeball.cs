using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Pokeball
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Generation of Introduction")]
        public int? GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}