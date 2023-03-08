using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class HuntingMethod
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}