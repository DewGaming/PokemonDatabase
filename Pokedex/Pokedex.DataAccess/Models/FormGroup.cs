using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class FormGroup
    {
        public int Id { get; set; }
        [Display(Name = "Form Group Name (Will Appear In Team Randomizer)"), StringLength(20), Required]
        public string Name { get; set; }
        [Display(Name = "Will this Form Group appear in the Team Randomizer"), Required]
        public bool AppearInTeamRandomizer { get; set; }
    }
}