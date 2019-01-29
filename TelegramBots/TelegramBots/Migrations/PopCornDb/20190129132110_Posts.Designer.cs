﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBots.Context;

namespace TelegramBots.Migrations.PopCornDb
{
    [DbContext(typeof(PopCornDbContext))]
    [Migration("20190129132110_Posts")]
    partial class Posts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataLayer.Models.Concert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AppleMusicLink");

                    b.Property<string>("Artist");

                    b.Property<string>("DeezerMusicLink");

                    b.Property<DateTime>("EventDate");

                    b.Property<string>("FacebookLink");

                    b.Property<string>("FullDescription");

                    b.Property<string>("GoogleMusicLink");

                    b.Property<string>("PhotoReport");

                    b.Property<string>("Poster");

                    b.Property<string>("ShortDescription");

                    b.Property<string>("SpotifyMusicLink");

                    b.Property<string>("TicketsLink");

                    b.Property<string>("Venue");

                    b.Property<string>("VideoInfo");

                    b.Property<string>("YoutubeMusicLink");

                    b.HasKey("Id");

                    b.ToTable("Concerts");
                });

            modelBuilder.Entity("DataLayer.Models.ConcertVisit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConcertId");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("VisitDate");

                    b.HasKey("Id");

                    b.HasIndex("ConcertId");

                    b.HasIndex("UserId");

                    b.ToTable("ConcertVisit");
                });

            modelBuilder.Entity("DataLayer.Models.MainInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AboutText");

                    b.Property<string>("CompanyName");

                    b.Property<string>("ConcertsText");

                    b.Property<string>("ContactsText");

                    b.Property<string>("HelloText");

                    b.Property<string>("SalesText");

                    b.Property<string>("TicketsText");

                    b.HasKey("Id");

                    b.ToTable("MainInfo");
                });

            modelBuilder.Entity("DataLayer.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ConcertId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Desription");

                    b.Property<bool>("IsCommonPost");

                    b.Property<string>("Link");

                    b.Property<DateTime?>("PublishDate");

                    b.Property<DateTime?>("ScheduleDate");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ConcertId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DataLayer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DataLayer.Models.UserSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConcertId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ConcertId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSubscription");
                });

            modelBuilder.Entity("DataLayer.Models.ConcertVisit", b =>
                {
                    b.HasOne("DataLayer.Models.Concert", "Concert")
                        .WithMany("ConcertVisits")
                        .HasForeignKey("ConcertId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLayer.Models.User", "User")
                        .WithMany("ConcertVisits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Post", b =>
                {
                    b.HasOne("DataLayer.Models.Concert", "Concert")
                        .WithMany("Posts")
                        .HasForeignKey("ConcertId");
                });

            modelBuilder.Entity("DataLayer.Models.UserSubscription", b =>
                {
                    b.HasOne("DataLayer.Models.Concert", "Concert")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ConcertId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLayer.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
