using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class MedicalRecord
{
    public int MedicalRecordId { get; set; }

    public int ElderId { get; set; }

    public DateOnly RecordDate { get; set; }

    public string? Diagnosis { get; set; }

    public string? Medications { get; set; }

    public string? Allergies { get; set; }

    public string? ChronicConditions { get; set; }

    public virtual Elder Elder { get; set; } = null!;
}
