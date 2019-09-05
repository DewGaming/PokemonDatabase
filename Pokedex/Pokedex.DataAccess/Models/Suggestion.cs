using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Suggestion   
    {
        public int Id { get; set; }
        [StringLength(500), Required, Display(Name = "Suggestion")]
        public string Name { get; set; }
        public string Commentor { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
    }
}