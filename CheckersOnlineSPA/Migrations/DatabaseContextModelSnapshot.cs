﻿// <auto-generated />
using CheckersOnlineSPA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CheckersOnlineSPA.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CheckersOnlineSPA.Data.Games", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("FirstPlayerId")
                        .HasColumnType("int");

                    b.Property<int>("SecondPlayerId")
                        .HasColumnType("int");

                    b.Property<int>("winner")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FirstPlayerId");

                    b.HasIndex("SecondPlayerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("CheckersOnlineSPA.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Picture")
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("varchar(24)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CheckersOnlineSPA.Data.Games", b =>
                {
                    b.HasOne("CheckersOnlineSPA.Data.User", "FirstPlayer")
                        .WithMany()
                        .HasForeignKey("FirstPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CheckersOnlineSPA.Data.User", "SecondPlayer")
                        .WithMany()
                        .HasForeignKey("SecondPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FirstPlayer");

                    b.Navigation("SecondPlayer");
                });
#pragma warning restore 612, 618
        }
    }
}
