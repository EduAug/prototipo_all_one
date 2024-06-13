using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace all_one_backend.Models;

public partial class AllOneDatabContext : DbContext
{
    public AllOneDatabContext()
    {
    }

    public AllOneDatabContext(DbContextOptions<AllOneDatabContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Topic");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TopicName).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.DisplayName).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasMany(d => d.Friends).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Friend",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("UserId", "FriendId");
                        j.ToTable("Friends");
                        j.HasIndex(new[] { "FriendId" }, "FriendID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("FriendId").HasColumnName("FriendID");
                    });

            entity.HasMany(d => d.Topics).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserTopic",
                    r => r.HasOne<Topic>().WithMany()
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("UserId", "TopicId");
                        j.ToTable("UserTopics");
                        j.HasIndex(new[] { "TopicId" }, "TopicID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("TopicId").HasColumnName("TopicID");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.Friends)
                .UsingEntity<Dictionary<string, object>>(
                    "Friend",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("UserId", "FriendId");
                        j.ToTable("Friends");
                        j.HasIndex(new[] { "FriendId" }, "FriendID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("FriendId").HasColumnName("FriendID");
                    });
        });
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.TopicId });
                entity.ToTable("Votes");
                entity.Property(e => e.VoteDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
