using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Pokeball   
    {
        public int Id { get; set; }
        [StringLength(20), Required]
        public string Name { get; set; }
    }
}