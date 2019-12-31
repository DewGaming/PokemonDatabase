using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class EggCycle   
    {
        public int Id { get; set; }
        [Required, Display(Name = "Egg Cycles")]
        public Int16 CycleCount { get; set; }
        [Required, NotMapped]
        public string CycleWithSteps
        {
            get
            {
                return string.Concat(CycleCount.ToString(), " (", CycleCount * 257, "-", CycleCount * 257 + 256, ")");
            }
        }
    }
}