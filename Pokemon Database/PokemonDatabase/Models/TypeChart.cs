using System;

namespace PokemonDatabase.Models
{
    public class TypeChart
    {
        public int ID { get; set; }
        public decimal Effective { get; set; }
        public int? AttackID { get; set; }
        public virtual Type Attack { get; set; }
        public int? DefendID { get; set; }
        public virtual Type Defend { get; set; }
    }
}