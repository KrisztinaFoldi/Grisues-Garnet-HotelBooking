﻿// <auto-generated />
using HotelBookingGarnet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotelBookingGarnet.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20190925122249_hotel2")]
    partial class hotel2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HotelBookingGarnet.Models.Hotel", b =>
                {
                    b.Property<long>("HotelId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("Description");

                    b.Property<string>("HotelName");

                    b.Property<int>("Price");

                    b.Property<string>("PropertyType");

                    b.Property<string>("Region");

                    b.Property<int>("StarRating");

                    b.Property<long>("UserId");

                    b.HasKey("HotelId");

                    b.HasIndex("UserId");

                    b.ToTable("Hotels");
                });

            modelBuilder.Entity("HotelBookingGarnet.Models.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Role");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HotelBookingGarnet.Models.Hotel", b =>
                {
                    b.HasOne("HotelBookingGarnet.Models.User")
                        .WithMany("Hotels")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
