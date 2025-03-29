using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Caregiver
{
    public int CaregiverId { get; set; }

    public int AccountId { get; set; }

    public int ExperienceYears { get; set; }

    public string? Specialty { get; set; }

    public string? Certification { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
