using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Stat
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Abbreviation { get; set; }
    }
}