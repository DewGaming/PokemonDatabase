using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class BaseHappiness   
    {
        public int Id { get; set; }
        [Required]
        public byte Happiness { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}