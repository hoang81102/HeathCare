using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<BookingTimeSlot> BookingTimeSlots { get; set; }

    public virtual DbSet<Caregiver> Caregivers { get; set; }

    public virtual DbSet<CaregiverAvailability> CaregiverAvailabilities { get; set; }

    public virtual DbSet<CaregiverSchedule> CaregiverSchedules { get; set; }

    public virtual DbSet<CustomerElder> CustomerElders { get; set; }

    public virtual DbSet<Elder> Elders { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }

    public virtual DbSet<ServiceFeedback> ServiceFeedbacks { get; set; }

    public virtual DbSet<Tracking> Trackings { get; set; }

    private string GetConnectionString()
    {
        string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ElderlyCareMVC", "appsettings.json");


        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath, true, true)
            .Build();
        return configuration["ConnectionStrings:MyStockDB"];
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__F267251E10D60828");

            entity.ToTable("Account");

            entity.HasIndex(e => e.RoleId, "IX_Account_RoleId");

            entity.HasIndex(e => e.AccountStatus, "IX_Account_Status");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E616427A0524A").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Account__F3DBC572E512CB91").IsUnique();

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
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BCDCCFF732D");

            entity.ToTable("Booking");

            entity.HasIndex(e => e.AccountId, "IX_Booking_AccountId");

            entity.HasIndex(e => e.CaregiverId, "IX_Booking_CaregiverId");

            entity.HasIndex(e => e.ElderId, "IX_Booking_ElderId");

            entity.HasIndex(e => e.Status, "IX_Booking_Status");

            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.BookingDateTime)
                .HasColumnType("datetime")
                .HasColumnName("bookingDateTime");
            entity.Property(e => e.CaregiverId).HasColumnName("caregiverId");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.RejectionReason)
                .HasColumnType("text")
                .HasColumnName("rejectionReason");
            entity.Property(e => e.ServiceId).HasColumnName("serviceId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__account__5165187F");

            entity.HasOne(d => d.Caregiver).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CaregiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__caregiv__534D60F1");

            entity.HasOne(d => d.Elder).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ElderId)
                .HasConstraintName("FK__Booking__elderId__5441852A");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__service__52593CB8");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("BookingDetails");

            entity.Property(e => e.BookingDate).HasColumnName("bookingDate");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.CaregiverName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("caregiverName");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("categoryName");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("customerName");
            entity.Property(e => e.ElderName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("elderName");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.RejectionReason)
                .HasColumnType("text")
                .HasColumnName("rejectionReason");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serviceName");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
        });

        modelBuilder.Entity<BookingTimeSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__BookingT__9C4A67130E3C59E2");

            entity.ToTable("BookingTimeSlot");

            entity.HasIndex(e => new { e.BookingDate, e.StartTime, e.EndTime }, "IX_BookingTimeSlot_BookingDate");

            entity.HasIndex(e => e.BookingId, "IX_BookingTimeSlot_BookingId");

            entity.Property(e => e.SlotId).HasColumnName("slotId");
            entity.Property(e => e.BookingDate).HasColumnName("bookingDate");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.StartTime).HasColumnName("startTime");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTimeSlots)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingTi__booki__571DF1D5");
        });

        modelBuilder.Entity<Caregiver>(entity =>
        {
            entity.HasKey(e => e.CaregiverId).HasName("PK__Caregive__085ECD45BB1B6C3D");

            entity.ToTable("Caregiver");

            entity.Property(e => e.CaregiverId).HasColumnName("caregiverId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Availability)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("availability");
            entity.Property(e => e.Certification)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("certification");
            entity.Property(e => e.ExperienceYears).HasColumnName("experienceYears");
            entity.Property(e => e.Specialty)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("specialty");

            entity.HasOne(d => d.Account).WithMany(p => p.Caregivers)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Caregiver__accou__48CFD27E");
        });

        modelBuilder.Entity<CaregiverAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__Caregive__BFEBC05551479314");

            entity.ToTable("CaregiverAvailability");

            entity.HasIndex(e => new { e.CaregiverId, e.DayOfWeek, e.StartTime, e.EndTime }, "IX_CaregiverAvailability_DayTime");

            entity.HasIndex(e => new { e.CaregiverId, e.DayOfWeek, e.StartTime, e.EndTime }, "UC_CaregiverSchedule").IsUnique();

            entity.Property(e => e.AvailabilityId).HasColumnName("availabilityId");
            entity.Property(e => e.CaregiverId).HasColumnName("caregiverId");
            entity.Property(e => e.DayOfWeek).HasColumnName("dayOfWeek");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("isAvailable");
            entity.Property(e => e.StartTime).HasColumnName("startTime");

            entity.HasOne(d => d.Caregiver).WithMany(p => p.CaregiverAvailabilities)
                .HasForeignKey(d => d.CaregiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Caregiver__careg__4D94879B");
        });

        modelBuilder.Entity<CaregiverSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CaregiverSchedules");

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.CaregiverId).HasColumnName("caregiverId");
            entity.Property(e => e.DayOfWeek).HasColumnName("dayOfWeek");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.ExperienceYears).HasColumnName("experienceYears");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.IsAvailable).HasColumnName("isAvailable");
            entity.Property(e => e.Specialty)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("specialty");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
        });

        modelBuilder.Entity<CustomerElder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CustomerElders");

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("customerName");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.ElderName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("elderName");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.MedicalNote)
                .HasColumnType("text")
                .HasColumnName("medicalNote");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Elder>(entity =>
        {
            entity.HasKey(e => e.ElderId).HasName("PK__Elder__6CB011A3DCD3719C");

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
                .HasConstraintName("FK__Elder__accountId__44FF419A");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FD2415529597");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.CaregiverProfessionalism).HasColumnName("caregiverProfessionalism");
            entity.Property(e => e.FeedbackDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("feedbackDate");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.OverallExperience).HasColumnName("overallExperience");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ServiceQuality).HasColumnName("serviceQuality");

            entity.HasOne(d => d.Booking).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__bookin__6754599E");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.MedicalRecordId).HasName("PK__MedicalR__14A80D056A8AA79A");

            entity.ToTable("MedicalRecord");

            entity.Property(e => e.MedicalRecordId).HasColumnName("medicalRecordId");
            entity.Property(e => e.Allergies)
                .HasColumnType("text")
                .HasColumnName("allergies");
            entity.Property(e => e.ChronicConditions)
                .HasColumnType("text")
                .HasColumnName("chronicConditions");
            entity.Property(e => e.Diagnosis)
                .HasColumnType("text")
                .HasColumnName("diagnosis");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.Medications)
                .HasColumnType("text")
                .HasColumnName("medications");
            entity.Property(e => e.RecordDate).HasColumnName("recordDate");

            entity.HasOne(d => d.Elder).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.ElderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MedicalRe__elder__6A30C649");
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Record__D825195E2C65FB1B");

            entity.ToTable("Record");

            entity.HasIndex(e => e.BookingId, "IX_Record_BookingId");

            entity.HasIndex(e => e.ElderId, "IX_Record_ElderId");

            entity.HasIndex(e => e.Status, "IX_Record_Status");

            entity.Property(e => e.RecordId).HasColumnName("recordId");
            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.ClockInTime)
                .HasColumnType("datetime")
                .HasColumnName("clockInTime");
            entity.Property(e => e.ClockOutTime)
                .HasColumnType("datetime")
                .HasColumnName("clockOutTime");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DietGuidelines)
                .HasColumnType("text")
                .HasColumnName("dietGuidelines");
            entity.Property(e => e.ElderId).HasColumnName("elderId");
            entity.Property(e => e.ExerciseGuidelines)
                .HasColumnType("text")
                .HasColumnName("exerciseGuidelines");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.OtherGuidelines)
                .HasColumnType("text")
                .HasColumnName("otherGuidelines");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Records)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Record__bookingI__5CD6CB2B");

            entity.HasOne(d => d.Elder).WithMany(p => p.Records)
                .HasForeignKey(d => d.ElderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Record__elderId__5BE2A6F2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98462A73290454");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__455070DFB074441D");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId).HasColumnName("serviceId");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
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

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Service__categor__4222D4EF");
        });

        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ServiceC__23CAF1D869AA0B97");

            entity.ToTable("ServiceCategory");

            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("categoryName");
        });

        modelBuilder.Entity<ServiceFeedback>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ServiceFeedback");

            entity.Property(e => e.BookingId).HasColumnName("bookingId");
            entity.Property(e => e.CaregiverName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("caregiverName");
            entity.Property(e => e.CaregiverProfessionalism).HasColumnName("caregiverProfessionalism");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("customerName");
            entity.Property(e => e.ElderName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("elderName");
            entity.Property(e => e.FeedbackDate)
                .HasColumnType("datetime")
                .HasColumnName("feedbackDate");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.OverallExperience).HasColumnName("overallExperience");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serviceName");
            entity.Property(e => e.ServiceQuality).HasColumnName("serviceQuality");
        });

        modelBuilder.Entity<Tracking>(entity =>
        {
            entity.HasKey(e => e.TrackingId).HasName("PK__Tracking__A81574EECA299EEF");

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
                .HasConstraintName("FK__Tracking__elderI__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
