using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Ability   
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
        [StringLength(150), Required]
        public string Description { get; set; }
    }
}