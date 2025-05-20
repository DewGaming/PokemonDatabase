using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Comment is required"), Display(Name = "Comment")]
        public string Name { get; set; }
        public int? CommentorId { get; set; }
        public User Commentor { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public bool IsAutomatedError { get; set; }
    }
}