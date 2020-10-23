using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Ability   
    {
        public int Id { get; set; }
        [StringLength(30), Required]
        public string Name { get; set; }
        [StringLength(300), Required]
        public string Description { get; set; }
    }
}