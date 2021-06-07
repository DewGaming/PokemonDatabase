using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Region
    {
        public int Id { get; set; }
        [StringLength(10), Required]
        public string Name { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}