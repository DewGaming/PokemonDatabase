using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class SpecialGrouping
    {
        [Display(Name = "Type of Special Group")]
        public int Id { get; set; }
        [StringLength(20), Display(Name = "Special Grouping Name"), Required]
        public string Name { get; set; }
    }
}