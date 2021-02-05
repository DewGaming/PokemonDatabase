using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class GenderRatio
    {
        public int Id { get; set; }
        [Required, Display(Name = "Male Ratio")]
        public double MaleRatio { get; set; }
        [Required, Display(Name = "Female Ratio")]
        public double FemaleRatio { get; set; }
    }
}