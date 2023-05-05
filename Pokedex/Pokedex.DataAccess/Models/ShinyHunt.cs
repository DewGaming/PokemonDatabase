using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class ShinyHunt
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required, Display(Name = "Pokémon Hunting")]
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }

        [StringLength(12), Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Required, Display(Name = "Game Hunting In")]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required, Display(Name = "Hunting Method")]
        public int HuntingMethodId { get; set; }
        public HuntingMethod HuntingMethod { get; set; }

        [Display(Name = "Shiny Charm")]
        public bool HasShinyCharm { get; set; }

        [Display(Name = "Community Day")]
        public bool DuringCommunityDay { get; set; }

        [Display(Name = "Using Lures")]
        public bool UsingLures { get; set; }

        [Display(Name = "Alpha Pokemon")]
        public bool IsAlpha { get; set; }

        [Display(Name = "Sparkling Power Sandwich Level"), Range(0, 3)]
        public int SparklingPowerLevel { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Captured")]
        public DateTime DateOfCapture { get; set; }

        [Display(Name = "Number of Phases")]
        public int Phases { get; set; }

        [Display(Name = "Number of Current Phase Encounters")]
        public int CurrentPhaseEncounters { get; set; }

        [Display(Name = "Number of Total Encounters")]
        public int TotalEncounters { get; set; }

        [Display(Name = "Pokéball Used")]
        public int? PokeballId { get; set; }
        public Pokeball Pokeball { get; set; }

        [Display(Name = "Mark")]
        public int? MarkId { get; set; }
        public Mark Mark { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Captured")]
        public bool IsCaptured { get; set; }
    }
}