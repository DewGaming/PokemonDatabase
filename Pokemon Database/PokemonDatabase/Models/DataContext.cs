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
    }
}