using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class Generation
    {
        public int Id { get; set; }
        [Display(Name = "Generation Name"), Required, NotMapped]
        public string GenerationName { get { return string.Concat("Generation ", Id); } }

        public override bool Equals(object obj)
        {
            if (!(obj is Generation item))
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