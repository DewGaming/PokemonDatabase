using System;
using System.ComponentModel.DataAnnotations;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.DataAccess.Models
{
    public class PokemonTypeDetail  
    {
        public int Id { get; set; }
        [Required]
        public virtual Pokemon Pokemon { get; set; }
        [Required]
        public virtual Models.Type PrimaryType { get; set; }
        public virtual Models.Type SecondaryType { get; set; }
    }
}