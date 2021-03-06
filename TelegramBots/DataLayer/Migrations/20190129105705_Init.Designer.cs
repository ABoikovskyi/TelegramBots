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
    [DbContext(typeof(PlayZoneDbContext))]
    [Migration("20190129105705_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataLayer.Models.UserRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContactFirstName");

                    b.Property<string>("ContactLastName");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Game");

                    b.Property<int?>("GameConsole");

                    b.Property<int?>("NumberOfPeople");

                    b.Property<DateTime?>("RequestDate");

                    b.Property<int>("Status");

                    b.Property<long>("UserId");

                    b.Property<string>("UserName");

                    b.Property<string>("UserPhone");

                    b.Property<int?>("ZoneId");

                    b.HasKey("Id");

                    b.ToTable("UserRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
