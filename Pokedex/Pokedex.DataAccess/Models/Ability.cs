using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Ability
    {
        public int Id { get; set; }
        [StringLength(30), Required]
        public string Name { get; set; }
        [StringLength(300), Required]
        public string Description { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Ability item))
            {
                return false;
            }

            return this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}