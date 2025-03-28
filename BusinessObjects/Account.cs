using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string Fullname { get; set; } = null!;

    public string? Address { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Hobby { get; set; }

    public string AccountStatus { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Caregiver> Caregivers { get; set; } = new List<Caregiver>();

    public virtual ICollection<Elder> Elders { get; set; } = new List<Elder>();

    public virtual Role Role { get; set; } = null!;
}
