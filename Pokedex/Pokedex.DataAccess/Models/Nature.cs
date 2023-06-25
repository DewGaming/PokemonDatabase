using System.ComponentModel.DataAnnotations;

namespace Pokedex.DataAccess.Models
{
    public class Nature
    {
        public Nature()
        { }

        public Nature(Nature nature)
        {
            Id = nature.Id;
            Name = nature.Name;
            RaisedStatId = nature.RaisedStatId;
            RaisedStat = nature.RaisedStat;
            LoweredStatId = nature.LoweredStatId;
            LoweredStat = nature.LoweredStat;
        }

        public int Id { get; set; }
        [StringLength(25), Required]
        public string Name { get; set; }
        public int? RaisedStatId { get; set; }
        public Stat RaisedStat { get; set; }
        public int? LoweredStatId { get; set; }
        public Stat LoweredStat { get; set; }
    }
}