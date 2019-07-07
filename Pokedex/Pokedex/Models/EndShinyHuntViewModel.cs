using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EndShinyHuntViewModel
    {
        public int ShinyHuntId { get; set; }

        public ShinyHunt ShinyHunt { get; set; }

        [Display(Name = "Was Hunt Successful")]
        public bool HuntSuccessful { get; set; }
    }
}