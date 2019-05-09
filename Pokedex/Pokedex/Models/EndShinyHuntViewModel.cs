using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class EndShinyHuntViewModel
    {
        public int shinyHuntId { get; set; }
        public ShinyHunt shinyHunt { get; set; }
        [Display(Name = "Was Hunt Successful")]
        public bool HuntSuccessful { get; set; }
    }
}