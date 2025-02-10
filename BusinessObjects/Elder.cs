using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Elder
{
    public int ElderId { get; set; }

    public int AccountId { get; set; }

    public string Fullname { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Hobby { get; set; }

    public string? MedicalNote { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();

    public virtual ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();
}
