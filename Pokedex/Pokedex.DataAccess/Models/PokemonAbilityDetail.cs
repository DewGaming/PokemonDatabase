using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class PokemonAbilityDetail  
    {
        public int Id { get; set; }
        [Required]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }
        [Display(Name = "Primary Ability"), Required]
        public int? PrimaryAbilityId { get; set; }
        public Ability PrimaryAbility { get; set; }
        [Display(Name = "Secondary Ability")]
        public int? SecondaryAbilityId { get; set; }
        public Ability SecondaryAbility { get; set; }
        [Display(Name = "Hidden Ability")]
        public int? HiddenAbilityId { get; set; }
        public Ability HiddenAbility { get; set; }
        [Display(Name = "Special Event Ability")]
        public int? SpecialEventAbilityId { get; set; }
        public Ability SpecialEventAbility { get; set; }
        [Required]
        public int GenerationId { get; set; }
        public Generation Generation { get; set; }
    }
}