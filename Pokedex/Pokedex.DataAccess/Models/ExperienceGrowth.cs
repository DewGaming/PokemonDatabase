using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ExperienceGrowth   
    {
        public int Id { get; set; }
        [StringLength(15), Required]
        public string Name { get; set; }
        [Required]
        public int ExpPointTotal { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}