using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Sweet
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}