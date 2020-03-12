using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class CommentViewModel : Comment
    {
        public List<string> Page { get; set; }

        public List<string> TypeOfComment { get; set; }
    }
}