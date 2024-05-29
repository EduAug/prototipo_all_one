﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using all_one_backend.Models;

#nullable disable

namespace all_one_backend.Migrations
{
    [DbContext(typeof(AllOneDatabContext))]
    [Migration("20240405234714_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Friend", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.Property<int>("FriendId")
                        .HasColumnType("int")
                        .HasColumnName("FriendID");

                    b.HasKey("UserId", "FriendId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "FriendId" }, "FriendID");

                    b.ToTable("Friends", (string)null);
                });

            modelBuilder.Entity("UserTopic", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.Property<int>("TopicId")
                        .HasColumnType("int")
                        .HasColumnName("TopicID");

                    b.HasKey("UserId", "TopicId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "TopicId" }, "TopicID");

                    b.ToTable("UserTopics", (string)null);
                });

            modelBuilder.Entity("all_one_backend.Models.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int>("Subscribers")
                        .HasColumnType("int");

                    b.Property<string>("TopicName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<int>("TotalVotes")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("Topic", (string)null);
                });

            modelBuilder.Entity("all_one_backend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<decimal?>("Latitude")
                        .HasPrecision(10, 8)
                        .HasColumnType("decimal(10,8)");

                    b.Property<decimal?>("Longitude")
                        .HasPrecision(11, 8)
                        .HasColumnType("decimal(11,8)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Friend", b =>
                {
                    b.HasOne("all_one_backend.Models.User", null)
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .IsRequired()
                        .HasConstraintName("Friends_ibfk_2");

                    b.HasOne("all_one_backend.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("Friends_ibfk_1");
                });

            modelBuilder.Entity("UserTopic", b =>
                {
                    b.HasOne("all_one_backend.Models.Topic", null)
                        .WithMany()
                        .HasForeignKey("TopicId")
                        .IsRequired()
                        .HasConstraintName("UserTopics_ibfk_2");

                    b.HasOne("all_one_backend.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("UserTopics_ibfk_1");
                });
#pragma warning restore 612, 618
        }
    }
}
