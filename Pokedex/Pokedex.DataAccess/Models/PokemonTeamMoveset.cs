using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonTeamMoveset
    {
        public int Id { get; set; }

        [Display(Name = "First Move")]
        public string FirstMove { get; set; }

        [Display(Name = "Second Move")]
        public string SecondMove { get; set; }

        [Display(Name = "Third Move")]
        public string ThirdMove { get; set; }

        [Display(Name = "Fourth Move")]
        public string FourthMove { get; set; }
    }
}