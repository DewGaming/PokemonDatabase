using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Type   
    {
        public int Id { get; set; }
        [StringLength(10), Required]
        public string Name { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}