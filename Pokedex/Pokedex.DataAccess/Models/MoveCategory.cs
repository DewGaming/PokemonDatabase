using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class MoveCategory
    {
        public int Id { get; set; }
        [Required]
        public string Category { get; set; }
    }
}