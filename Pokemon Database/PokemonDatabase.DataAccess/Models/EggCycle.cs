using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonDatabase.DataAccess.Models
{
    public class EggCycle   
    {
        public int Id { get; set; }
        [Required]
        public Int16 CycleCount { get; set; }
    }
}