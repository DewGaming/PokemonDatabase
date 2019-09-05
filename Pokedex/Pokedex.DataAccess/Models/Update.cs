using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Update   
    {
        public int Id { get; set; }
        [StringLength(500), Required]
        public string Name { get; set; }
    }
}