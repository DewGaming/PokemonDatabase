using System;

namespace PokemonDatabase.Models
{
    public class CaptureRate   
    {
        public int ID { get; set; }
        public Int16 CatchRate { get; set; }
        public decimal ChanceOfCapture { get; set; }
    }
}