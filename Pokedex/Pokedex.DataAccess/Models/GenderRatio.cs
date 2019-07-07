using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class GenderRatio   
    {
        public int Id { get; set; }
        [Required]
        public double MaleRatio { get; set; }
        [Required]
        public double FemaleRatio { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}