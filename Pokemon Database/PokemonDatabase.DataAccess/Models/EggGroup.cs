using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class EggGroup   
    {
        public int Id { get; set; }
        [StringLength(15)]
        [Required]
        public string Name { get; set; }
    }
}