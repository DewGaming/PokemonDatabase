using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Season
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}