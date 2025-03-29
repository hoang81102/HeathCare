using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class ServiceFeedback
{
    public int BookingId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string ElderName { get; set; } = null!;

    public string CaregiverName { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public int? Rating { get; set; }

    public int? CaregiverProfessionalism { get; set; }

    public int? ServiceQuality { get; set; }

    public int? OverallExperience { get; set; }

    public string? Note { get; set; }

    public DateTime? FeedbackDate { get; set; }
}
