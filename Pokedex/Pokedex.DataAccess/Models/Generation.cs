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
    }
}