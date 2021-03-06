﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using butterystrava.Models;

namespace butterystrava.Migrations
{
    [DbContext(typeof(ButteryContext))]
    partial class ButteryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("butterystrava.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("DateExpiresAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateExpiresIn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateRefreshed")
                        .HasColumnType("TEXT");

                    b.Property<long>("ExpiresAt")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExpiresIn")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
