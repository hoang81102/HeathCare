using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class CaregiverSchedule
{
    public int AccountId { get; set; }

    public string Fullname { get; set; } = null!;

    public int CaregiverId { get; set; }

    public int ExperienceYears { get; set; }

    public string? Specialty { get; set; }

    public int? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool? IsAvailable { get; set; }
}
