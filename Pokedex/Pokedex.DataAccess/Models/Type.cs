using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Type
    {
        public int Id { get; set; }
        [StringLength(10), Required]
        public string Name { get; set; }
        [Display(Name = "Generation Introduced In"), Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}