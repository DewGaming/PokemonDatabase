using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ExperienceGrowth   
    {
        public int Id { get; set; }
        [StringLength(15), Required, Display(Name = "Experience Growth Name")]
        public string Name { get; set; }
        [Required, Display(Name = "Exp Point Total")]
        public int ExpPointTotal { get; set; }
    }
}