using System;

namespace PokemonDatabase.Models
{
    public class PokemonAbilityDetail  
    {
        public int ID { get; set; }
        public string PokemonID { get; set; }
        public virtual Pokemon Pokemon { get; set; }
        public int PrimaryAbilityID { get; set; }
        public virtual Ability PrimaryAbility { get; set; }
        public int? SecondaryAbilityID { get; set; }
        public virtual Ability SecondaryAbility { get; set; }
        public int? HiddenAbilityID { get; set; }
        public virtual Ability HiddenAbility { get; set; }
    }
}