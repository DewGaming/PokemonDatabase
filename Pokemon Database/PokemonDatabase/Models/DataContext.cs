using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PokemonDatabase.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        public DbSet<Ability> Abilities { get; set; }

        public DbSet<Type> Types { get; set; }

        public DbSet<EggGroup> EggGroups { get; set; }

        public DbSet<BaseHappiness> BaseHappiness { get; set; }

        public DbSet<CaptureRate> CaptureRates { get; set; }

        public DbSet<Classification> Classifications { get; set; }

        public DbSet<EggCycle> EggCycles { get; set; }
    }
}