using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class CaptureRate   
    {
        public int Id { get; set; }
        [Required, Display(Name = "Catch Rate")]
        public Int16 CatchRate { get; set; }
    }
}