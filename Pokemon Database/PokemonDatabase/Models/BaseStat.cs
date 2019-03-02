using System;

namespace PokemonDatabase.Models
{
    public class BaseStat   
    {
        public int ID { get; set; }
        public Int16 Health { get; set; }
        public Int16 Attack { get; set; }
        public Int16 Defense { get; set; }
        public Int16 SpecialAttack { get; set; }
        public Int16 SpecialDefense { get; set; }
        public Int16 Speed { get; set; }
        public Int16 StatTotal { get; set; }
        public string PokemonID { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }
}