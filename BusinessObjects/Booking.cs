using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Booking
{
    public int BookingId { get; set; }

    public int AccountId { get; set; }

    public int ServiceId { get; set; }

    public int CaregiverId { get; set; }

    public int? ElderId { get; set; }

    public DateTime BookingDateTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Caregiver Caregiver { get; set; } = null!;

    public virtual Elder? Elder { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();

    public virtual Service Service { get; set; } = null!;
}
