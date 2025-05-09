﻿using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int BookingId { get; set; }

    public string? Note { get; set; }

    public int? Rating { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public int? CaregiverProfessionalism { get; set; }

    public int? ServiceQuality { get; set; }

    public int? OverallExperience { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
