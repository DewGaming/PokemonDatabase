using Pokedex.DataAccess.Models;
using System.Collections.Generic;

namespace Pokedex.Models
{
    /// <summary>
    /// The class that is used to represent the comments page's view model.
    /// </summary>
    public class AllCommentsViewModel : Comment
    {
        /// <summary>
        /// Gets or sets a list of all comment types.
        /// </summary>
        public List<CommentCategory> AllCategories { get; set; }

        /// <summary>
        /// Gets or sets a list of all comments.
        /// </summary>
        public List<Comment> AllComments { get; set; }
    }
}
