using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class Type   
    {
        public int Id { get; set; }
        [StringLength(10)]
        [Required]
        public string Name { get; set; }
    }
}