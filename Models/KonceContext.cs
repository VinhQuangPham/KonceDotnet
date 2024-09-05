using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Konce.Models;

public partial class KonceContext : DbContext
{
    public KonceContext()
    {
    }

    public KonceContext(DbContextOptions<KonceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventGuest> EventGuests { get; set; }

    public virtual DbSet<NiceUser> NiceUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     => optionsBuilder.UseSqlServer("Server=localhost;Database=Konce;User Id=sa;Password=dockerStrongPwd123;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EVENT__3214EC27F639ACC1");

            entity.ToTable("EVENT");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Completed).HasDefaultValue(false);
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_date");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EisenhowerLevel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Eisenhower_level");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("End_time");
            entity.Property(e => e.HangoutLink)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Hangout_link");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("Start_time");
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Updated_date");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Events)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_USER_ID");
        });

        modelBuilder.Entity<EventGuest>(entity =>
        {
            entity.HasKey(e => e.EventGuestId).HasName("PK__EVENT_GU__15C82B91E7EB605A");

            entity.ToTable("EVENT_GUEST");

            entity.Property(e => e.EventGuestId).HasColumnName("Event_Guest_ID");
            entity.Property(e => e.EventId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Event_ID");
            entity.Property(e => e.GuestId).HasColumnName("Guest_ID");
            entity.Property(e => e.ResponseStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("Response_status");

            entity.HasOne(d => d.Event).WithMany(p => p.EventGuests)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_Event_ID");

            entity.HasOne(d => d.Guest).WithMany(p => p.EventGuests)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("FK_USER_ID_FOR_GUEST");
        });

        modelBuilder.Entity<NiceUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__NICE_USE__206D9190AFA00070");

            entity.ToTable("NICE_USER");

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.BufferTime).HasColumnName("Buffer_time");
            entity.Property(e => e.Chronotype)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Phone_number");
            entity.Property(e => e.UserCredentials)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("User_credentials");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("User_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
