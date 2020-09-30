using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokeballCatchModifierDetail  
    {
        public int Id { get; set; }
        [Required]
        public int PokeballId { get; set; }
        public Pokeball Pokeball { get; set; }
        [Display(Name = "Effect"), Required]
        public string Effect { get; set; }
        [Display(Name = "Catch Modifer"), Required]
        public float CatchModifier { get; set; }
    }
}