using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pokedex.DataAccess.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Ability> Abilities { get; set; }
        public DbSet<BaseHappiness> BaseHappiness { get; set; }
        public DbSet<BaseStat> BaseStats { get; set; }
        public DbSet<CaptureRate> CaptureRates { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<EggCycle> EggCycles { get; set; }
        public DbSet<EggGroup> EggGroups { get; set; }
        public DbSet<Evolution> Evolutions { get; set; }
        public DbSet<EvolutionMethod> EvolutionMethods { get; set; }
        public DbSet<EVYield> EVYields { get; set; }
        public DbSet<ExperienceGrowth> ExperienceGrowths { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<GenderRatio> GenderRatios { get; set; }
        public DbSet<Generation> Generations { get; set; }
        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonAbilityDetail> PokemonAbilityDetails { get; set; }
        public DbSet<PokemonEggGroupDetail> PokemonEggGroupDetails { get; set; }
        public DbSet<PokemonFormDetail> PokemonFormDetails { get; set; }
        public DbSet<PokemonTypeDetail> PokemonTypeDetails { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<TypeChart> TypeCharts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ShinyHuntingTechnique> ShinyHuntingTechniques { get; set; }
        public DbSet<ShinyHunt> ShinyHunts { get; set; }
        public DbSet<PokemonLegendaryDetail> PokemonLegendaryDetails { get; set; }
        public DbSet<LegendaryType> LegendaryTypes { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FormItem> FormItems { get; set; }
        public DbSet<PokemonTeam> PokemonTeams { get; set; }
        public DbSet<PokemonTeamDetail> PokemonTeamDetails { get; set; }
        public DbSet<PokemonTeamEV> PokemonTeamEVs { get; set; }
        public DbSet<PokemonTeamIV> PokemonTeamIVs { get; set; }
    }
}