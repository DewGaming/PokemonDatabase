using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class EggGroup
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
    }
}