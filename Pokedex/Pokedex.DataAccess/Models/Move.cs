using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Move   
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
        [StringLength(300), Required]
        public string Description { get; set; }
    }
}