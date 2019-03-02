﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokemonDatabase.Models;

namespace PokemonDatabase.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190301234929_AddTypeChart")]
    partial class AddTypeChart
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PokemonDatabase.Models.Ability", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Abilities");
                });

            modelBuilder.Entity("PokemonDatabase.Models.BaseHappiness", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("Happiness");

                    b.HasKey("ID");

                    b.ToTable("BaseHappiness");
                });

            modelBuilder.Entity("PokemonDatabase.Models.BaseStat", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Attack");

                    b.Property<short>("Defense");

                    b.Property<short>("Health");

                    b.Property<string>("PokemonID");

                    b.Property<short>("SpecialAttack");

                    b.Property<short>("SpecialDefense");

                    b.Property<short>("Speed");

                    b.Property<short>("StatTotal");

                    b.HasKey("ID");

                    b.HasIndex("PokemonID");

                    b.ToTable("BaseStats");
                });

            modelBuilder.Entity("PokemonDatabase.Models.CaptureRate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("CatchRate");

                    b.Property<decimal>("ChanceOfCapture");

                    b.HasKey("ID");

                    b.ToTable("CaptureRates");
                });

            modelBuilder.Entity("PokemonDatabase.Models.Classification", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Classifications");
                });

            modelBuilder.Entity("PokemonDatabase.Models.EggCycle", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("CycleCount");

                    b.HasKey("ID");

                    b.ToTable("EggCycles");
                });

            modelBuilder.Entity("PokemonDatabase.Models.EggGroup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("EggGroups");
                });

            modelBuilder.Entity("PokemonDatabase.Models.EVYield", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Attack");

                    b.Property<short>("Defense");

                    b.Property<short>("EVTotal");

                    b.Property<short>("Health");

                    b.Property<string>("PokemonID");

                    b.Property<short>("SpecialAttack");

                    b.Property<short>("SpecialDefense");

                    b.Property<short>("Speed");

                    b.HasKey("ID");

                    b.HasIndex("PokemonID");

                    b.ToTable("EVYields");
                });

            modelBuilder.Entity("PokemonDatabase.Models.ExperienceGrowth", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ExpPointTotal");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("ExperienceGrowths");
                });

            modelBuilder.Entity("PokemonDatabase.Models.Form", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("PokemonDatabase.Models.GenderRatio", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("FemaleRatio");

                    b.Property<double>("MaleRatio");

                    b.HasKey("ID");

                    b.ToTable("GenderRatios");
                });

            modelBuilder.Entity("PokemonDatabase.Models.Generation", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Games");

                    b.Property<string>("Region");

                    b.HasKey("ID");

                    b.ToTable("Generations");
                });

            modelBuilder.Entity("PokemonDatabase.Models.Pokemon", b =>
                {
                    b.Property<string>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BaseHappinessID");

                    b.Property<int>("CaptureRateID");

                    b.Property<int>("ClassificationID");

                    b.Property<int>("EggCycleID");

                    b.Property<int>("ExpYield");

                    b.Property<int>("ExperienceGrowthID");

                    b.Property<int>("GenderRatioID");

                    b.Property<string>("GenerationID");

                    b.Property<decimal>("Height");

                    b.Property<string>("Name");

                    b.Property<decimal>("Weight");

                    b.HasKey("ID");

                    b.HasIndex("BaseHappinessID");

                    b.HasIndex("CaptureRateID");

                    b.HasIndex("ClassificationID");

                    b.HasIndex("EggCycleID");

                    b.HasIndex("ExperienceGrowthID");

                    b.HasIndex("GenderRatioID");

                    b.HasIndex("GenerationID");

                    b.ToTable("Pokemon");
                });

            modelBuilder.Entity("PokemonDatabase.Models.Type", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Types");
                });

            modelBuilder.Entity("PokemonDatabase.Models.TypeChart", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AttackID");

                    b.Property<int?>("DefendID");

                    b.Property<decimal>("Effective");

                    b.HasKey("ID");

                    b.HasIndex("AttackID");

                    b.HasIndex("DefendID");

                    b.ToTable("TypeChart");
                });

            modelBuilder.Entity("PokemonDatabase.Models.BaseStat", b =>
                {
                    b.HasOne("PokemonDatabase.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonID");
                });

            modelBuilder.Entity("PokemonDatabase.Models.EVYield", b =>
                {
                    b.HasOne("PokemonDatabase.Models.Pokemon", "Pokemon")
                        .WithMany()
                        .HasForeignKey("PokemonID");
                });

            modelBuilder.Entity("PokemonDatabase.Models.Pokemon", b =>
                {
                    b.HasOne("PokemonDatabase.Models.BaseHappiness", "BaseHappiness")
                        .WithMany()
                        .HasForeignKey("BaseHappinessID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PokemonDatabase.Models.CaptureRate", "CaptureRate")
                        .WithMany()
                        .HasForeignKey("CaptureRateID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PokemonDatabase.Models.Classification", "Classification")
                        .WithMany()
                        .HasForeignKey("ClassificationID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PokemonDatabase.Models.EggCycle", "EggCycle")
                        .WithMany()
                        .HasForeignKey("EggCycleID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PokemonDatabase.Models.ExperienceGrowth", "ExperienceGrowth")
                        .WithMany()
                        .HasForeignKey("ExperienceGrowthID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PokemonDatabase.Models.GenderRatio", "GenderRatio")
                        .WithMany()
                        .HasForeignKey("GenderRatioID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PokemonDatabase.Models.Generation", "Generation")
                        .WithMany()
                        .HasForeignKey("GenerationID");
                });

            modelBuilder.Entity("PokemonDatabase.Models.TypeChart", b =>
                {
                    b.HasOne("PokemonDatabase.Models.Type", "Attack")
                        .WithMany()
                        .HasForeignKey("AttackID");

                    b.HasOne("PokemonDatabase.Models.Type", "Defend")
                        .WithMany()
                        .HasForeignKey("DefendID");
                });
#pragma warning restore 612, 618
        }
    }
}
