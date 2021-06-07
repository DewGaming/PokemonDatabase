using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Pokeball
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
        [Required, Display(Name = "Generation Introduced To Normal Gameplay")]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}