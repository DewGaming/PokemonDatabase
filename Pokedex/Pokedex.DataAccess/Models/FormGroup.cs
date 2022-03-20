using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class FormGroup
    {
        public int Id { get; set; }
        [Display(Name = "Form Group Name (Will Appear In Team Randomizer)"), StringLength(20), Required]
        public string Name { get; set; }
    }
}