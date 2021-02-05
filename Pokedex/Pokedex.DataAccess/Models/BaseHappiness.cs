using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class BaseHappiness
    {
        public int Id { get; set; }
        [Required]
        public byte Happiness { get; set; }
    }
}