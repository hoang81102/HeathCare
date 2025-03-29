using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class BookingTimeSlot
{
    public int SlotId { get; set; }

    public int BookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
