using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Status   
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Effect { get; set; }
    }
}