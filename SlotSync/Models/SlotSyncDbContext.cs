using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SlotSync.Models;

public partial class SlotSyncDbContext : DbContext
{
    public SlotSyncDbContext()
    {
    }

    public SlotSyncDbContext(DbContextOptions<SlotSyncDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED8B0AFDA7");

            entity.HasIndex(e => new { e.SlotId, e.BookingDate }, "UQ_Bookings_Slot_Date").IsUnique();

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Slot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__SlotId__01142BA1");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__UserId__02084FDA");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Slots__0A124AAFDC48447A");

            entity.HasOne(d => d.Venue).WithMany(p => p.Slots)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Slots__VenueId__74AE54BC");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__1788CC4C9190A966");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__A9D105340494B9BB").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.PasswordHash).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__VENUES__3C57E5F204633D5E");

            entity.ToTable("VENUES");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.VenueType).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
