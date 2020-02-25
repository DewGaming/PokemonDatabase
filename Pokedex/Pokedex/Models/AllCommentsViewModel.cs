using System.Collections.Generic;
using Pokedex.DataAccess.Models;

namespace Pokedex.Models
{
    public class AllCommentsViewModel : Comment
    {
        public List<string> CommentTypes { get; set; }

        public List<Comment> AllComments { get; set; }
    }
}