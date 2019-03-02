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

        public DbSet<ExperienceGrowth> ExperienceGrowths { get; set; }

        public DbSet<GenderRatio> GenderRatios { get; set; }

        public DbSet<Generation> Generations { get; set; }

        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<BaseStat> BaseStats { get; set; }
        public DbSet<EVYield> EVYields { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<TypeChart> TypeChart { get; set; }
        public DbSet<PokemonTypeDetail> PokemonTypeDetails { get; set; }
        public DbSet<PokemonAbilityDetail> PokemonAbilityDetails { get; set; }
    }
}