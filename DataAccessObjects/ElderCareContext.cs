using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using BusinessObjects;

namespace BusinessObjects;

public partial class ElderCareContext : DbContext
{
    public ElderCareContext()
    {
    }

    public ElderCareContext(DbContextOptions<ElderCareContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Elder> Elders { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Tracking> Trackings { get; set; }

    private string GetConnectionString()
    {
        string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ElderlyCareMVC", "appsettings.json");


        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath, true, true)
            .Build();
        return configuration["ConnectionStrings:DefaultConnectionString"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__F267251E8EA20C9D");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E61645B9F7971").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Account__F3DBC572853F6CD6").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.AccountStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("accountStatus");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.Hobby)
                .HasColumnType("text")
                .HasColumnName("hobby");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__roleId__3C69FB99");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BCDA7EC6824");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.BookingDateTime)
                .HasColumnType("datetime")
                .HasColumnName("bookingDateTime");
            entity.Property(e => e.CaregiverId).HasColumnName("caregiverId");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.ServiceId).HasColumnName("serviceId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.BookingAccounts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__account__44FF419A");

            entity.HasOne(d => d.Caregiver).WithMany(p => p.BookingCaregivers)
                .HasForeignKey(d => d.CaregiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__caregiv__46E78A0C");

            entity.HasOne(d => d.Elder).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ElderId)
                .HasConstraintName("FK__Booking__elderId__47DBAE45");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__service__45F365D3");
        });

        modelBuilder.Entity<Elder>(entity =>
        {
            entity.HasKey(e => e.ElderId).HasName("PK__Elder__6CB011A3F33EF0ED");

            entity.ToTable("Elder");

            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.Hobby)
                .HasColumnType("text")
                .HasColumnName("hobby");
            entity.Property(e => e.MedicalNote)
                .HasColumnType("text")
                .HasColumnName("medicalNote");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");

            entity.HasOne(d => d.Account).WithMany(p => p.Elders)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Elder__accountId__3F466844");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FD24A3E3ADEE");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Booking).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__bookin__534D60F1");
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Record__D825195E7BAFD727");

            entity.ToTable("Record");

            entity.Property(e => e.RecordId).HasColumnName("recordId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Records)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Record__bookingI__4CA06362");

            entity.HasOne(d => d.Elder).WithMany(p => p.Records)
                .HasForeignKey(d => d.ElderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Record__elderId__4BAC3F29");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98462A2E46D633");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__455070DF00CD1D2D");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId).HasColumnName("serviceId");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serviceName");
        });

        modelBuilder.Entity<Tracking>(entity =>
        {
            entity.HasKey(e => e.TrackingId).HasName("PK__Tracking__A81574EE1748F010");

            entity.ToTable("Tracking");

            entity.Property(e => e.TrackingId).HasColumnName("trackingId");
            entity.Property(e => e.BloodPressure)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("bloodPressure");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.Elder).WithMany(p => p.Trackings)
                .HasForeignKey(d => d.ElderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tracking__elderI__4F7CD00D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
