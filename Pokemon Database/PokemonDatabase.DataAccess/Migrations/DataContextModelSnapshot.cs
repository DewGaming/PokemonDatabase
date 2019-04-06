﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokemonDatabase.DataAccess.Models;

namespace PokemonDatabase.DataAccess.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Ability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("Abilities");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.BaseHappiness", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("Happiness");

                    b.HasKey("Id");

                    b.ToTable("BaseHappiness");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.BaseStat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Attack");

                    b.Property<short>("Defense");

                    b.Property<short>("Health");

                    b.Property<string>("PokemonId");

                    b.Property<short>("SpecialAttack");

                    b.Property<short>("SpecialDefense");

                    b.Property<short>("Speed");

                    b.Property<short>("StatTotal");

                    b.HasKey("Id");

                    b.HasIndex("PokemonId");

                    b.ToTable("BaseStats");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.CaptureRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("CatchRate");

                    b.Property<decimal>("ChanceOfCapture");

                    b.HasKey("Id");

                    b.ToTable("CaptureRates");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Classification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.ToTable("Classifications");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.EggCycle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("CycleCount");

                    b.HasKey("Id");

                    b.ToTable("EggCycles");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.EggGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.HasKey("Id");

                    b.ToTable("EggGroups");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Evolution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EvolutionDetails")
                        .HasMaxLength(200);

                    b.Property<int>("EvolutionMethodId");

                    b.Property<string>("EvolutionPokemonId")
                        .IsRequired();

                    b.Property<string>("PreevolutionPokemonId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("EvolutionMethodId");

                    b.HasIndex("EvolutionPokemonId");

                    b.HasIndex("PreevolutionPokemonId");

                    b.ToTable("Evolutions");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.EvolutionMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("EvolutionMethods");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.EVYield", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Attack");

                    b.Property<short>("Defense");

                    b.Property<short>("EVTotal");

                    b.Property<short>("Health");

                    b.Property<string>("PokemonId");

                    b.Property<short>("SpecialAttack");

                    b.Property<short>("SpecialDefense");

                    b.Property<short>("Speed");

                    b.HasKey("Id");

                    b.HasIndex("PokemonId");

                    b.ToTable("EVYields");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.ExperienceGrowth", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ExpPointTotal");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.HasKey("Id");

                    b.ToTable("ExperienceGrowths");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Form", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.GenderRatio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("FemaleRatio");

                    b.Property<double>("MaleRatio");

                    b.HasKey("Id");

                    b.ToTable("GenderRatios");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Generation", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("Games")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("IsArchived");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasMaxLength(6);

                    b.HasKey("Id");

                    b.ToTable("Generations");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Pokemon", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(6);

                    b.Property<int>("BaseHappinessId");

                    b.Property<int>("CaptureRateId");

                    b.Property<int>("ClassificationId");

                    b.Property<int>("EggCycleId");

                    b.Property<int>("ExpYield");

                    b.Property<int>("ExperienceGrowthId");

                    b.Property<int>("GenderRatioId");

                    b.Property<string>("GenerationId")
                        .IsRequired();

                    b.Property<decimal>("Height");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<decimal>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("BaseHappinessId");

                    b.HasIndex("CaptureRateId");

                    b.HasIndex("ClassificationId");

                    b.HasIndex("EggCycleId");

                    b.HasIndex("ExperienceGrowthId");

                    b.HasIndex("GenderRatioId");

                    b.HasIndex("GenerationId");

                    b.ToTable("Pokemon");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonAbilityDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("HiddenAbilityId");

                    b.Property<string>("PokemonId")
                        .IsRequired();

                    b.Property<int>("PrimaryAbilityId");

                    b.Property<int?>("SecondaryAbilityId");

                    b.HasKey("Id");

                    b.HasIndex("HiddenAbilityId");

                    b.HasIndex("PokemonId");

                    b.HasIndex("PrimaryAbilityId");

                    b.HasIndex("SecondaryAbilityId");

                    b.ToTable("PokemonAbilityDetails");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonEggGroupDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PokemonId")
                        .IsRequired();

                    b.Property<int>("PrimaryEggGroupId");

                    b.Property<int?>("SecondaryEggGroupId");

                    b.HasKey("Id");

                    b.HasIndex("PokemonId");

                    b.HasIndex("PrimaryEggGroupId");

                    b.HasIndex("SecondaryEggGroupId");

                    b.ToTable("PokemonEggGroupDetails");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonFormDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AltFormPokemonId")
                        .IsRequired();

                    b.Property<int>("FormId");

                    b.Property<string>("OriginalPokemonId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AltFormPokemonId");

                    b.HasIndex("FormId");

                    b.HasIndex("OriginalPokemonId");

                    b.ToTable("PokemonFormDetails");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonTypeDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PokemonId")
                        .IsRequired();

                    b.Property<int>("PrimaryTypeId");

                    b.Property<int?>("SecondaryTypeId");

                    b.HasKey("Id");

                    b.HasIndex("PokemonId");

                    b.HasIndex("PrimaryTypeId");

                    b.HasIndex("SecondaryTypeId");

                    b.ToTable("PokemonTypeDetails");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Type", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Types");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.TypeChart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AttackId");

                    b.Property<int>("DefendId");

                    b.Property<decimal>("Effective");

                    b.HasKey("Id");

                    b.HasIndex("AttackId");

                    b.HasIndex("DefendId");

                    b.ToTable("TypeCharts");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EmailAddress")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsArchived");

                    b.Property<bool>("IsOwner");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.BaseStat", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonId");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Evolution", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.EvolutionMethod", "EvolutionMethod")
                        .WithMany()
                        .HasForeignKey("EvolutionMethodId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "EvolutionPokemon")
                        .WithMany()
                        .HasForeignKey("EvolutionPokemonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "PreevolutionPokemon")
                        .WithMany()
                        .HasForeignKey("PreevolutionPokemonId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.EVYield", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonId");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.Pokemon", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.BaseHappiness", "BaseHappiness")
                        .WithMany()
                        .HasForeignKey("BaseHappinessId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.CaptureRate", "CaptureRate")
                        .WithMany()
                        .HasForeignKey("CaptureRateId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Classification", "Classification")
                        .WithMany()
                        .HasForeignKey("ClassificationId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.EggCycle", "EggCycle")
                        .WithMany()
                        .HasForeignKey("EggCycleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.ExperienceGrowth", "ExperienceGrowth")
                        .WithMany()
                        .HasForeignKey("ExperienceGrowthId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.GenderRatio", "GenderRatio")
                        .WithMany()
                        .HasForeignKey("GenderRatioId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Generation", "Generation")
                        .WithMany()
                        .HasForeignKey("GenerationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonAbilityDetail", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Ability", "HiddenAbility")
                        .WithMany()
                        .HasForeignKey("HiddenAbilityId");

                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Ability", "PrimaryAbility")
                        .WithMany()
                        .HasForeignKey("PrimaryAbilityId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Ability", "SecondaryAbility")
                        .WithMany()
                        .HasForeignKey("SecondaryAbilityId");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonEggGroupDetail", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.EggGroup", "PrimaryEggGroup")
                        .WithMany()
                        .HasForeignKey("PrimaryEggGroupId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.EggGroup", "SecondaryEggGroup")
                        .WithMany()
                        .HasForeignKey("SecondaryEggGroupId");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonFormDetail", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "AltFormPokemon")
                        .WithMany()
                        .HasForeignKey("AltFormPokemonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Form", "Form")
                        .WithMany()
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "OriginalPokemon")
                        .WithMany()
                        .HasForeignKey("OriginalPokemonId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.PokemonTypeDetail", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Type", "PrimaryType")
                        .WithMany()
                        .HasForeignKey("PrimaryTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Type", "SecondaryType")
                        .WithMany()
                        .HasForeignKey("SecondaryTypeId");
                });

            modelBuilder.Entity("PokemonDatabase.DataAccess.Models.TypeChart", b =>
                {
                    b.HasOne("PokemonDatabase.DataAccess.Models.Type", "Attack")
                        .WithMany()
                        .HasForeignKey("AttackId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PokemonDatabase.DataAccess.Models.Type", "Defend")
                        .WithMany()
                        .HasForeignKey("DefendId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
