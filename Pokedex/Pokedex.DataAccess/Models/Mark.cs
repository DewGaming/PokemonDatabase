using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Mark
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, Display(Name="Name Addition")]
        public string NameAddition { get; set; }
        [Display(Name = "Generation of Introduction")]
        public int? GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}