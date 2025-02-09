using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Record
{
    public int RecordId { get; set; }

    public int ElderId { get; set; }

    public string Description { get; set; } = null!;

    public int BookingId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? LastUpdated { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Elder Elder { get; set; } = null!;
}
