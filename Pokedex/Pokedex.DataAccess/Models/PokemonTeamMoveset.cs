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

        public bool CheckMove(string move)
        {
            bool found = false;
            if (this.FirstMove == move)
            {
                found = true;
            }
            else if (this.SecondMove == move)
            {
                found = true;
            }
            else if (this.ThirdMove == move)
            {
                found = true;
            }
            else if (this.FourthMove == move)
            {
                found = true;
            }

            return found;
        }
    }
}