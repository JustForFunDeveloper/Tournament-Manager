﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TournamentManager.Core.Data;

namespace TournamentManager.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200823130407_Position_TeamUpdate")]
    partial class Position_TeamUpdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.Match", b =>
                {
                    b.Property<int>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.HasKey("MatchId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.MatchResult", b =>
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

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.Round", b =>
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

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.SoloTeamMatchResult", b =>
                {
                    b.Property<int>("SoloTeamMatchResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Result")
                        .HasColumnType("REAL");

                    b.Property<double>("Round")
                        .HasColumnType("REAL");

                    b.Property<int?>("TeamMatchId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("SoloTeamMatchResultId");

                    b.HasIndex("TeamMatchId");

                    b.ToTable("SoloTeamMatchResults");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.TeamMatch", b =>
                {
                    b.Property<int>("TeamMatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.HasKey("TeamMatchId");

                    b.ToTable("TeamMatches");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.TeamMatchResult", b =>
                {
                    b.Property<int>("TeamMatchResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Result")
                        .HasColumnType("REAL");

                    b.Property<int?>("TeamMatchId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TeamName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserNames")
                        .HasColumnType("TEXT");

                    b.HasKey("TeamMatchResultId");

                    b.HasIndex("TeamMatchId");

                    b.ToTable("TeamMatchResults");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.User", b =>
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

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.UserMatch", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MatchId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "MatchId");

                    b.HasIndex("MatchId");

                    b.ToTable("UserMatches");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.UserTeamMatch", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamMatchId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "TeamMatchId");

                    b.HasIndex("TeamMatchId");

                    b.ToTable("UserTeamMatches");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.MatchResult", b =>
                {
                    b.HasOne("TournamentManager.DataModels.DbModels.Match", null)
                        .WithMany("MatchResults")
                        .HasForeignKey("MatchId");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.Round", b =>
                {
                    b.HasOne("TournamentManager.DataModels.DbModels.User", null)
                        .WithMany("Rounds")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.SoloTeamMatchResult", b =>
                {
                    b.HasOne("TournamentManager.DataModels.DbModels.TeamMatch", null)
                        .WithMany("SoloTeamMatchResults")
                        .HasForeignKey("TeamMatchId");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.TeamMatchResult", b =>
                {
                    b.HasOne("TournamentManager.DataModels.DbModels.TeamMatch", null)
                        .WithMany("TeamMatchResults")
                        .HasForeignKey("TeamMatchId");
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.UserMatch", b =>
                {
                    b.HasOne("TournamentManager.DataModels.DbModels.Match", "Match")
                        .WithMany("UserMatches")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TournamentManager.DataModels.DbModels.User", "User")
                        .WithMany("UserMatches")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TournamentManager.DataModels.DbModels.UserTeamMatch", b =>
                {
                    b.HasOne("TournamentManager.DataModels.DbModels.TeamMatch", "TeamMatch")
                        .WithMany("UserTeamMatches")
                        .HasForeignKey("TeamMatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TournamentManager.DataModels.DbModels.User", "User")
                        .WithMany("UserTeamMatches")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
