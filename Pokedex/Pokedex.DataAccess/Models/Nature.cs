using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Nature
    {
        public int Id { get; set; }
        [StringLength(25), Required]
        public string Name { get; set; }
    }
}