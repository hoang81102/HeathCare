using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class CustomerElder
{
    public int AccountId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int ElderId { get; set; }

    public string ElderName { get; set; } = null!;

    public DateOnly? Birthdate { get; set; }

    public string? MedicalNote { get; set; }
}
