using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class EggCycle   
    {
        public int Id { get; set; }
        [Required, Display(Name = "Egg Cycles")]
        public Int16 CycleCount { get; set; }
    }
}