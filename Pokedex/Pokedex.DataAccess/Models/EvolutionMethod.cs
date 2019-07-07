using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class EvolutionMethod   
    {
        public int Id { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
    }
}