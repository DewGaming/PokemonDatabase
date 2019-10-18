using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ShinyHuntingTechnique   
    {
        public int Id { get; set; }
        [Display(Name = "Name of Technique"), Required]
        public string Name { get; set; }
        [Display(Name = "Description of Technique"), Required]
        public string Technique { get; set; }
    }
}