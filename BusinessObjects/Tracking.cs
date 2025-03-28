using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Tracking
{
    public int TrackingId { get; set; }

    public int ElderId { get; set; }

    public DateOnly Date { get; set; }

    public decimal? Weight { get; set; }

    public string? BloodPressure { get; set; }

    public virtual Elder Elder { get; set; } = null!;
}
