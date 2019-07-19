﻿// <auto-generated />
using System;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Migrations
{
    [DbContext(typeof(IdrinkDbContext))]
    [Migration("20190719092302_Log")]
    partial class Log
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataLayer.Models.Idrink.DrinkHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalInfo");

                    b.Property<string>("Beverage");

                    b.Property<DateTime>("DrinkTime");

                    b.Property<float?>("Latitude");

                    b.Property<float?>("Longitude");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("DrinkHistory");
                });

            modelBuilder.Entity("DataLayer.Models.Idrink.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("LogDate");

                    b.Property<string>("Message");

                    b.Property<string>("StackTrace");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("DataLayer.Models.Idrink.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("SubscribedOnId");

                    b.Property<int>("SubscriberId");

                    b.HasKey("Id");

                    b.HasIndex("SubscribedOnId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("DataLayer.Models.Idrink.User", b =>
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

            modelBuilder.Entity("DataLayer.Models.Idrink.DrinkHistory", b =>
                {
                    b.HasOne("DataLayer.Models.Idrink.User", "User")
                        .WithMany("DrinkHistory")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Idrink.Log", b =>
                {
                    b.HasOne("DataLayer.Models.Idrink.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLayer.Models.Idrink.Subscription", b =>
                {
                    b.HasOne("DataLayer.Models.Idrink.User", "SubscribedOn")
                        .WithMany()
                        .HasForeignKey("SubscribedOnId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLayer.Models.Idrink.User", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
