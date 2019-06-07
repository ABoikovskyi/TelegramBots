﻿// <auto-generated />
using System;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Migrations.FestivalDb
{
    [DbContext(typeof(FestivalDbContext))]
    [Migration("20190607132250_RemoveViber")]
    partial class RemoveViber
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataLayer.Models.Festival.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Facebook");

                    b.Property<string>("Image");

                    b.Property<string>("Instagram");

                    b.Property<string>("Itunes");

                    b.Property<string>("Name");

                    b.Property<string>("Twitter");

                    b.Property<string>("Website");

                    b.Property<string>("Youtube");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Festival", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CampingInfo");

                    b.Property<string>("Contacts");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("HowToGetTo");

                    b.Property<byte[]>("Map");

                    b.Property<string>("Name");

                    b.Property<string>("Rules");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Festivals");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ArtistId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Desription");

                    b.Property<bool>("IsCommonPost");

                    b.Property<string>("Link");

                    b.Property<DateTime?>("PublishDate");

                    b.Property<DateTime?>("ScheduleDate");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArtistId");

                    b.Property<DateTime>("EndDate");

                    b.Property<int>("StageId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("StageId");

                    b.ToTable("Schedule");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Stage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int>("FestivalId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("FestivalId");

                    b.ToTable("Stages");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatId");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.UserSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArtistId");

                    b.Property<int?>("ScheduleId");

                    b.Property<int>("Type");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSubscription");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Post", b =>
                {
                    b.HasOne("DataLayer.Models.Festival.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId");
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Schedule", b =>
                {
                    b.HasOne("DataLayer.Models.Festival.Artist", "Artist")
                        .WithMany("ScheduleData")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLayer.Models.Festival.Stage", "Stage")
                        .WithMany("ScheduleData")
                        .HasForeignKey("StageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Festival.Stage", b =>
                {
                    b.HasOne("DataLayer.Models.Festival.Festival", "Festival")
                        .WithMany("Stages")
                        .HasForeignKey("FestivalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Festival.UserSubscription", b =>
                {
                    b.HasOne("DataLayer.Models.Festival.Artist", "Artist")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLayer.Models.Festival.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId");

                    b.HasOne("DataLayer.Models.Festival.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
