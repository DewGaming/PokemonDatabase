using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class ShinyHuntingViewModel
    {
        public List<ShinyHunt> InProgressHunts { get; set; }

        public List<ShinyHunt> CompletedHunts { get; set; }

        public List<ShinyHunt> FailedHunts { get; set; }
    }
}