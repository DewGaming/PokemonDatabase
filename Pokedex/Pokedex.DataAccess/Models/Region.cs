using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Region
    {
        public int Id { get; set; }
        [StringLength(10), Required]
        public string Name { get; set; }
        [Display(Name = "Generation of Introduction"), Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}