using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Message   
    {
        public int Id { get; set; }
        [StringLength(100), Required, Display(Name = "Title")]
        public string MessageTitle { get; set; }
        [StringLength(1000), Required, Display(Name = "Message")]
        public string MessageContent { get; set; }
        [Required]
        public int? SenderId { get; set; }
        public User Sender { get; set; }
        [Required]
        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }
    }
}