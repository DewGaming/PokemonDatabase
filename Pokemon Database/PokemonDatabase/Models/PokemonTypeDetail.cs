using System;

namespace PokemonDatabase.Models
{
    public class PokemonTypeDetail  
    {
        public int ID { get; set; }
        public string PokemonID { get; set; }
        public virtual Pokemon Pokemon { get; set; }
        public int PrimaryTypeID { get; set; }
        public virtual PokemonDatabase.Models.Type PrimaryType { get; set; }
        public int? SecondaryTypeID { get; set; }
        public virtual PokemonDatabase.Models.Type SecondaryType { get; set; }
    }
}