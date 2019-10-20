using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.DataAccess.Models
{
    public class CaptureRate   
    {
        public int Id { get; set; }
        [Required]
        public Int16 CatchRate { get; set; }
    }
}