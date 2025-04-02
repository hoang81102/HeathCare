using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class CaregiverAvailability
{
    public int AvailabilityId { get; set; }

    public int CaregiverId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual Caregiver Caregiver { get; set; } = null!;
}
