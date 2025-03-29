using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class BookingDetail
{
    public int BookingId { get; set; }

    public string Status { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string ElderName { get; set; } = null!;

    public string CaregiverName { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public DateOnly BookingDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? RejectionReason { get; set; }
}
