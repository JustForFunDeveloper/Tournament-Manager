﻿// <auto-generated />
using System;
using HandicapBewerb.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HandicapBewerb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.Match", b =>
                {
                    b.Property<int>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MatchId");

                    b.HasIndex("UserId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.MatchResult", b =>
                {
                    b.Property<int>("MatchResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MatchId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Result")
                        .HasColumnType("REAL");

                    b.Property<double>("Round")
                        .HasColumnType("REAL");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("MatchResultId");

                    b.HasIndex("MatchId");

                    b.ToTable("MatchResults");
                });

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.Round", b =>
                {
                    b.Property<int>("RoundId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<double>("Points")
                        .HasColumnType("REAL");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("RoundId");

                    b.HasIndex("UserId");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.Match", b =>
                {
                    b.HasOne("HandicapBewerb.DataModels.DbModels.User", null)
                        .WithMany("Matches")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.MatchResult", b =>
                {
                    b.HasOne("HandicapBewerb.DataModels.DbModels.Match", null)
                        .WithMany("MatchResults")
                        .HasForeignKey("MatchId");
                });

            modelBuilder.Entity("HandicapBewerb.DataModels.DbModels.Round", b =>
                {
                    b.HasOne("HandicapBewerb.DataModels.DbModels.User", null)
                        .WithMany("Rounds")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
